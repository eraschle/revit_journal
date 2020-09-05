using RevitJournal.Revit.Journal;
using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Revit
{
    public class RevitProcess : IDisposable
    {
        private bool disposedValue;

        internal Process Process { get; private set; }

        private int ProcessId { get; set; }

        internal RevitArguments Arguments { get; private set; }

        public RevitProcess(RevitArguments arguments)
        {
            Arguments = arguments;
        }

        public Task<bool> RunTaskAsync(TaskJournalFile journalProcess, CancellationToken cancellation)
        {
            return Task.Run(() => Run(journalProcess), cancellation);
        }

        public bool Run(TaskJournalFile journalProcess)
        {
            using (Process = CreateProccess(journalProcess))
            {
                if (Process.Start() == false)
                {
                    Debug.WriteLine("Revit Process not started:" + Environment.NewLine +
                        "FileName  : " + Process.StartInfo.FileName + Environment.NewLine +
                        "WorkingDir: " + Process.StartInfo.WorkingDirectory + Environment.NewLine +
                        "Arguments : " + Process.StartInfo.Arguments);
                    return false;
                }
                ProcessId = Process.Id;
                return Process.WaitForExit((int)Arguments.Timeout.TotalMilliseconds);
            }
        }

        internal void KillProcess()
        {
            if (Process is null) { return; }

            KillChildren();
            try { Process.Kill(); }
            catch (Exception e)
            {
                Debug.WriteLine($"Process.Kill Exception: {e.Message}");
            }

            try { Process.Dispose(); }
            catch (Exception e)
            {
                Debug.WriteLine($"Process.Close Exception: {e.Message}");
            }
            Process = null;
        }

        private void KillChildren()
        {
            using (var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + ProcessId))
            {
                foreach (var managmentObject in searcher.Get())
                {
                    var subPid = Convert.ToInt32(managmentObject["ProcessID"]);
                    try
                    {
                        var proc = Process.GetProcessById(subPid);
                        if (proc.HasExited == false)
                        {
                            proc.Kill();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine($"SubProcess.Kill Exception: {e.Message}");
                    }
                }
            }
        }

        #region Create Process and Arguments

        internal Process CreateProccess(TaskJournalFile journalProcess)
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

        #endregion
    }
}
