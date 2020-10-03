using Autodesk.Revit.DB.Structure;
using RevitJournal.Revit.Journal;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.System;

namespace RevitJournal.Revit
{
    public class RevitProcess : IDisposable
    {
        public event EventHandler<EventArgs> ProcessFinished;

        protected virtual void OnProcessFinished()
        {
            ProcessFinished?.Invoke(this, EventArgs.Empty);
        }

        private bool disposedValue;

        internal Process Process { get; private set; }

        private int ProcessId { get; set; }

        private Dictionary<int, string> ChildProcessIds { get; set; } = new Dictionary<int, string>();

        internal RevitArguments Arguments { get; private set; }

        public RevitProcess(RevitArguments arguments)
        {
            Arguments = arguments;
        }

        public Task<bool> RunTaskAsync(TaskJournalFile journal, CancellationToken cancellation)
        {
            return Task.Run(() => Run(journal), cancellation);
        }

        public bool Run(TaskJournalFile journal)
        {
            if (journal is null) { throw new ArgumentNullException(nameof(journal)); }

            using (Process = CreateProccess(journal))
            {
                if (Process.Start() == false)
                {
                    var message = new StringBuilder();
                    message.AppendLine("Revit Process not started:");
                    message.AppendLine($"FileName  : {Process.StartInfo.FileName}");
                    message.AppendLine($"WorkingDir: {Process.StartInfo.WorkingDirectory}");
                    message.AppendLine($"Arguments : {Process.StartInfo.Arguments}");
                    DebugUtils.Line<RevitProcess>(message.ToString());
                    return false;
                }
                ProcessId = Process.Id;
                ChildProcessIds.Add(ProcessId, journal.GetFileName());
                CollectChildProcessIds();
                return Process.WaitForExit(Arguments.TimeoutTime);
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        internal void KillProcess()
        {
            if (Process is null) { return; }

            try
            {
                Process.Kill();
            }
            catch (Exception ex)
            {
                //DebugUtils.Exception<RevitProcess>(ex, GetDebugMessage(ProcessId, "Kill"));
            }

            try
            {
                Process.Dispose();
            }
            catch (Exception ex)
            {
                //DebugUtils.Exception<RevitProcess>(ex, GetDebugMessage(ProcessId, "Dispose"));
            }
            Process = null;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void KillChildren()
        {
            var processIds = new List<int>(ChildProcessIds.Keys);
            foreach (var processId in processIds)
            {
                try
                {
                    var process = Process.GetProcessById(processId);
                    if (process.HasExited)
                    {
                        DebugUtils.Line<RevitProcess>(GetDebugMessage(processId, "Exited"));
                    }
                    else
                    {
                        DebugUtils.Line<RevitProcess>(GetDebugMessage(processId, "Kill"));
                        process.Kill();
                    }
                    ChildProcessIds.Remove(processId);
                }
                catch (Exception ex)
                {
                    DebugUtils.Exception<RevitProcess>(ex, GetDebugMessage(processId, string.Empty));
                    ChildProcessIds.Remove(processId);
                }
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private async void CollectChildProcessIds()
        {
            await Task.Run(() =>
            {
                var running = true;
                while (running)
                {
                    using (var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + ProcessId))
                    {
                        foreach (var managmentObject in searcher.Get())
                        {
                            var processId = Convert.ToInt32(managmentObject["ProcessID"], CultureInfo.CurrentCulture);
                            if (ChildProcessIds.ContainsKey(processId)) { continue; }

                            var process = Process.GetProcessById(processId);
                            ChildProcessIds.Add(processId, process.ProcessName);
                            //DebugUtils.Line<RevitProcess>(GetDebugMessage(processId, "Collect"));
                        }
                    }
                    WaitForContinue();
                    try
                    {
                        running = Process.HasExited == false;
                    }
                    catch (Exception)
                    {
                        running = false;
                    }
                }
            }).ConfigureAwait(false);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async void WaitChildProcessesExited()
        {
            await Task.Run(() =>
            {
                ChildProcessIds.Remove(ProcessId);
                while (ChildProcessIds.Count == 0)
                {
                    var processIds = new List<int>(ChildProcessIds.Keys);
                    foreach (var processId in processIds)
                    {
                        if(processId == ProcessId) { continue; }
                        try
                        {
                            var process = Process.GetProcessById(processId);
                            if (process.HasExited == false)
                            {
                                DebugUtils.Line<RevitProcess>(GetDebugMessage(processId, "NOT Exited"));
                                continue;
                            }

                            //DebugUtils.Line<RevitProcess>(GetDebugMessage(processId, "Exited"));
                            ChildProcessIds.Remove(processId);
                        }
                        catch (Exception ex)
                        {
                            //DebugUtils.Exception<RevitProcess>(ex, GetDebugMessage(processId, string.Empty));
                            ChildProcessIds.Remove(processId);
                        }
                    }
                    WaitForContinue();
                }
                OnProcessFinished();
            }).ConfigureAwait(false);
        }

        private static void WaitForContinue()
        {
            var time = TimeSpan.FromSeconds(2);
            Task.Delay(time).Wait();
        }

        private string GetDebugMessage(int processId, string suffix)
        {
            var processName = "NO NAME";
            var processKind = "UNKNOWN";
            if (ChildProcessIds.ContainsKey(processId))
            {
                processKind = $"Child of {ChildProcessIds[ProcessId]}";
                processName = ChildProcessIds[processId];
            }
            if (processId == ProcessId)
            {
                processKind = "Main";
            }
            var message = $"{processName} [{processId}] ({processKind})";
            if (string.IsNullOrWhiteSpace(suffix) == false)
            {
                message = $"{message}: {suffix}";
            }
            return message;
        }

        #region Create Process and Arguments

        private Process CreateProccess(TaskJournalFile journalProcess)
        {
            var process = new Process();
            process.StartInfo.FileName = Arguments.RevitExecutable;
            process.StartInfo.WorkingDirectory = Arguments.WorkingDirectory;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            if (Arguments.StartMinimized)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }
            process.StartInfo.Arguments = StartArguments(journalProcess);
            return process;
        }

        internal string StartArguments(TaskJournalFile journalProcess)
        {
            var argument = CreateStartArgument(journalProcess);
            argument = CreateLanguageArgument(argument);
            argument = CreateMinimized(argument);
            argument = CreateSplashArgument(argument);
            return argument.ToString();
        }

        private StringBuilder CreateStartArgument(TaskJournalFile journalProcess)
        {
            var args = new StringBuilder();
            args.Append(journalProcess.FullPath);
            return args;
        }

        private StringBuilder CreateLanguageArgument(StringBuilder args)
        {
            args.Append(" ");
            args.Append(Arguments.LanguageArgument);
            args.Append(Arguments.RevitApp.Language);
            return args;
        }

        private StringBuilder CreateMinimized(StringBuilder args)
        {
            if (Arguments.StartMinimized)
            {
                args.Append(" ");
                args.Append(Arguments.StartMinimizedArgument);
            }
            return args;
        }

        private StringBuilder CreateSplashArgument(StringBuilder args)
        {
            if (Arguments.ShowSplash == false)
            {
                args.Append(" ");
                args.Append(Arguments.ShowSplashArgument);
            }
            return args;
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Process?.Dispose();
                }
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RevitProcess()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

    }
}
