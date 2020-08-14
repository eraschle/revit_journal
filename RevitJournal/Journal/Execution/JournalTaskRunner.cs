using RevitJournal.Revit;
using RevitJournal.Tasks;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Journal.Execution
{
    public class JournalTaskRunner
    {
        private RevitProcess Process;

        private readonly JournalResult Result;

        private ProcessJournalFile JournalProcess { get { return Result.JournalProcess; } }

        private RevitTask JournalTask { get { return Result.JournalTask; } }

        private readonly IProgress<JournalResult> Progress;

        public JournalTaskRunner(RevitTask journalTask, IProgress<JournalResult> progress)
        {
            Progress = progress;
            Result = new JournalResult(journalTask);
        }

        //private void SetupEvents(JournalRevitCreator creator)
        //{
        //    creator.JournalCreated += new JournalRevitCreatorHandler(OnJournalRevitCreated);
        //}

        //private void CleanEvents(JournalRevitCreator creator)
        //{
        //    creator.JournalCreated -= OnJournalRevitCreated;
        //}

        internal async Task CreateTask(TaskOptions options, CancellationToken cancellation)
        {
            JournalTask.PreExecution(options.Backup);
            JournalTask.CreateJournalProcess(options.Common);
            //SetupEvents(creator);
            using (var taskCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
            {
                taskCancellation.Token.Register(CancelAction);
                Report(ResultStatus.Started);
                using (Process = new RevitProcess(options.Arguments))
                {
                    var processTask = await Process.RunTaskAsync(JournalProcess, taskCancellation.Token);
                    if (processTask == false)
                    {
                        Report(ResultStatus.Timeout);
                    }
                }
                Process = null;
                JournalTask.PostExecution(Result);
            }
            //CleanEvents(creator);
            Report(ResultStatus.Finish);
            var reportOptions = options.Report;
            if (reportOptions.LogResults)
            {
                //if (TaskOption.LogError && Result.HasError(out var error))
                //{
                //    JournalError.Write(error);
                //}
                if (reportOptions.LogSuccess)
                {
                    JournalResult.Write(Result);
                }
            }
        }

        private void CancelAction()
        {
            Report(ResultStatus.Cancel);
            if (Process is null) { return; }

            Process.KillProcess();
            Debug.WriteLine(JournalProcess.Name + ": Killed Process [CancelAction]");
        }

        //private void OnJournalRevitCreated(object sender, JournalRevitCreatedEventArgs args)
        //{
        //    if (JournalProcess is null
        //        || JournalProcess.IsSame(args.JournalProcessPath) == false) { return; }

        //    Result.JournalRevit = args.JournalRevit;
        //    Report(ResultStatus.Running);
        //    if (Result.HasError())
        //    {
        //        Report(ResultStatus.Error);
        //        if (Process is null) { return; }

        //        Process.KillProcess();
        //        Debug.WriteLine(JournalProcess.Name + ": Killed Process [OnJournalRevitCreated]");
        //    }
        //}

        internal void Report(int status)
        {
            if (ResultStatus.IsStatus(status) == false) { return; }

            Result.SetStatus(status);
            Progress.Report(Result);
        }
    }
}
