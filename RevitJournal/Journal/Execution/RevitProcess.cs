using DataSource.Model.Product;
using RevitJournal.Revit;
using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Journal.Execution
{
    public class RevitProcess : IDisposable
    {
        private const string LanguageArgument = "/language ";
        private const string MinimizedArgument = "/min ";
        private const string SplashArgument = "/nosplash ";

        internal Process Process { get; private set; }

        internal RevitApp RevitApp { get; private set; }

        public RevitProcess(RevitApp revitApp)
        {
            RevitApp = revitApp;
        }

        public Task<bool> RunTaskAsync(JournalProcessFile journalProcess, TimeSpan timeout, CancellationToken cancellation)
        {
            return Task.Run(() => Run(journalProcess, timeout), cancellation);
        }

        public bool Run(JournalProcessFile journalProcess, TimeSpan timeout)
        {
            var exited = false;
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
                var pid = Process.Id;
                exited = Process.WaitForExit((int)timeout.TotalMilliseconds);
                if (exited)
                {
                    KillChildren(pid);
                }
                else
                {
                    Debug.WriteLine($"WaitForExit False");
                    KillProcess();
                }
            }
            return exited;
        }

        internal void KillProcess()
        {
            if (Process is null) { return; }


            try { Process.CloseMainWindow(); }
            catch (Exception e)
            {
                Debug.WriteLine($"Process.CloseMainWindow Exception: {e.Message}");
            }

            KillChildren(Process.Id);
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
        }

        private void KillChildren(int pid)
        {
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            foreach (var managmentObject in searcher.Get())
            {
                var subPid = Convert.ToInt32(managmentObject["ProcessID"]);
                //KillChildren(subPid);
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

        #region Create Process and Arguments

        internal Process CreateProccess(JournalProcessFile journalProcess)
        {
            var process = new Process();
            process.StartInfo.FileName = RevitApp.AppFile.FullPath;
            process.StartInfo.WorkingDirectory = RevitApp.AppFile.ParentFolder;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            if (RevitApp.StartMinimized)
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            }
            process.StartInfo.Arguments = StartArguments(journalProcess);
            return process;
        }

        internal string StartArguments(JournalProcessFile journalProcess)
        {
            var argument = CreateStartArgument(journalProcess);
            argument = CreateLanguageArgument(argument);
            argument = CreateMinimized(argument);
            argument = CreateSplashArgument(argument);
            return argument.ToString();
        }

        private StringBuilder CreateStartArgument(JournalProcessFile journalProcess)
        {
            var args = new StringBuilder();
            args.Append(journalProcess.FullPath);
            return args;
        }

        private StringBuilder CreateLanguageArgument(StringBuilder args)
        {
            args.Append(" ");
            args.Append(LanguageArgument);
            args.Append(RevitApp.Language);
            return args;
        }

        private StringBuilder CreateMinimized(StringBuilder args)
        {
            if (RevitApp.StartMinimized)
            {
                args.Append(" ");
                args.Append(MinimizedArgument);
            }
            return args;
        }

        private StringBuilder CreateSplashArgument(StringBuilder args)
        {
            if (RevitApp.ShowSlash == false)
            {
                args.Append(" ");
                args.Append(SplashArgument);
            }
            return args;
        }

        public void Dispose()
        {
            ((IDisposable)Process).Dispose();
            Process = null;
        }

        #endregion
    }
}
