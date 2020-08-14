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

        private JournalProcessFile JournalProcess { get { return Result.JournalProcess; } }
        private RevitTask JournalTask { get { return Result.JournalTask; } }
        private JournalOption TaskOption { get { return JournalTask.TaskOption; } }

        private readonly IProgress<JournalResult> Progress;

        public JournalTaskRunner(RevitTask journalTask, IProgress<JournalResult> progress)
        {
            Progress = progress;
            Result = new JournalResult(journalTask);
        }

        private void SetupEvents(JournalRevitCreator creator)
        {
            creator.JournalCreated += new JournalRevitCreatorHandler(OnJournalRevitCreated);
        }

        private void CleanEvents(JournalRevitCreator creator)
        {
            creator.JournalCreated -= OnJournalRevitCreated;
        }

        internal async Task CreateTask(JournalRevitCreator creator, CancellationToken cancellation)
        {
            JournalTask.PreExecution(TaskOption);
            JournalTask.CreateJournalProcess(creator.JournalDirectory);
            SetupEvents(creator);
            using (var taskCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
            {
                taskCancellation.Token.Register(CancelAction);
                Report(ResultStatus.Started);
                using (Process = new RevitProcess(TaskOption.RevitApp))
                {
                    var processTask = await Process.RunTaskAsync(JournalProcess, TaskOption.TaskTimeout, taskCancellation.Token);
                    if (processTask == false)
                    {
                        Report(ResultStatus.Timeout);
                    }
                }
                Process = null;
                JournalTask.PostExecution(Result);
            }
            CleanEvents(creator);
            Report(ResultStatus.Finish);
            if (TaskOption.LogResults)
            {
                if (TaskOption.LogError && Result.HasError(out var error))
                {
                    JournalError.Write(error);
                }
                if (TaskOption.LogSuccess)
                {
                    JournalResult.Write(Result);
                }
            }
            creator.AddFinishedTask(Result.JournalRevit);
        }

        private void CancelAction()
        {
            Report(ResultStatus.Cancel);
            if (Process is null) { return; }

            Process.KillProcess();
            Debug.WriteLine(JournalProcess.Name + ": Killed Process [CancelAction]");
        }

        private void OnJournalRevitCreated(object sender, JournalRevitCreatedEventArgs args)
        {
            if (JournalProcess is null
                || JournalProcess.IsSame(args.JournalProcessPath) == false) { return; }

            Result.JournalRevit = args.JournalRevit;
            Report(ResultStatus.Running);
            if (Result.HasError())
            {
                Report(ResultStatus.Error);
                if (Process is null) { return; }

                Process.KillProcess();
                Debug.WriteLine(JournalProcess.Name + ": Killed Process [OnJournalRevitCreated]");
            }
        }

        internal void Report(int status)
        {
            if (ResultStatus.IsStatus(status) == false) { return; }

            Result.SetStatus(status);
            Progress.Report(Result);
        }
    }
}
