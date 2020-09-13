using DataSource.Model.FileSystem;
using RevitAction;
using RevitAction.Action;
using RevitAction.Report;
using RevitAction.Report.Message;
using RevitJournal.Report;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
using RevitJournal.Tasks.Actions;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Tasks
{
    public class TaskUnitOfWork : IReportReceiver, IEquatable<TaskUnitOfWork>
    {
        private static readonly NullRecordeJournalFile nullRecorde = new NullRecordeJournalFile();
        private static readonly NullTaskJournalFile nullTask = new NullTaskJournalFile();
        private static readonly ITaskAction nullAction = new NullTaskAction();

        public RevitTask Task { get; private set; }

        public TaskOptions Options { get; private set; }

        public TaskAppStatus Status { get; private set; } = new TaskAppStatus();

        public TaskJournalFile TaskJournal { get; set; } = nullTask;

        public RecordeJournalFile RecordeJournal { get; set; } = nullRecorde;

        public TaskReportManager ReportManager { get; private set; }

        public RevitProcess Process { get; set; }

        public IProgress<TaskUnitOfWork> Progress { get; set; }

        public TaskUnitOfWork(RevitTask task, TaskOptions options)
        {
            Task = task;
            Options = options;
            ReportManager = new TaskReportManager(this);
            Status.SetStatus(TaskAppStatus.Initial);
        }

        public string TaskId
        {
            get { return Task.SourceFile.FullPath; }
        }

        private TaskJournalFile CreateTaskJournal()
        {
            var journal = Options.JournalDirectory;
            return TaskJournalCreator.Create(Task, journal);
        }

        public void DeleteJournalProcess()
        {
            if (TaskJournal is null) { return; }

            TaskJournal.Delete();
        }

        public ITaskAction CurrentAction { get; private set; } = nullAction;

        public int ExecutedActions { get; private set; } = 0;

        public Guid GetNextAction(Guid actionId)
        {
            var nextActionId = Guid.Empty;
            if (ActionManager.IsOpenAction(actionId))
            {
                nextActionId = ActionManager.JournalActionId;
            }
            else
            {
                if (ActionManager.IsJournalAction(actionId))
                {
                    actionId = ActionManager.OpenActionId;
                }
                if (Task.HasNextAction(actionId, out var nextAction))
                {
                    nextActionId = nextAction.Id;
                }
            }
            return nextActionId;
        }

        public void MakeReport(ReportMessage report)
        {
            if (report is null) { return; }

            SetNewAction(report);
            Status.SetStatus(GetTaskStatus(report));
            ReportManager.AddReport(report);
            switch (report.Kind)
            {
                case ReportKind.Status:
                    if (ActionManager.IsOpenAction(report.ActionId)
                        || ActionManager.IsSaveAction(report.ActionId))
                    {
                        var resultFile = CreateFile<RevitFamilyFile>(report);
                        Task.ResultFile = resultFile;
                    }
                    else if (ActionManager.IsJournalAction(report.ActionId))
                    {
                        var journalFile = CreateFile<RecordeJournalFile>(report);
                        RecordeJournal = journalFile;
                    }
                    break;
                case ReportKind.Error:
                    KillProcess();
                    break;
                case ReportKind.Warning:
                default:
                    break;
            }
            Progress?.Report(this);
        }

        private void SetNewAction(ReportMessage report)
        {
            if (CurrentAction.Id.Equals(report.ActionId) == false
                && Task.HasActionById(report.ActionId, out var action))
            {
                ExecutedActions += 1;
                CurrentAction = action;
            }
        }

        private int GetTaskStatus(ReportMessage report)
        {
            var status = TaskAppStatus.Started;
            switch (report.Kind)
            {
                case ReportKind.Status:
                case ReportKind.Warning:
                    status = TaskAppStatus.Running;
                    break;
                case ReportKind.Error:
                    status = TaskAppStatus.Error;
                    break;
                default:
                    break;
            }
            return status;
        }

        private TFile CreateFile<TFile>(ReportMessage report) where TFile : AFile, new()
        {
            return new TFile { FullPath = report.Message };
        }

        #region Process

        public void KillProcess()
        {
            if (Process is null) { return; }

            try
            {
                Process.KillProcess();
                Process.Dispose();
            }
            finally
            {
                Process = null;
            }
        }

        public void CancelProcess()
        {
            ReportStatus(TaskAppStatus.Cancel);
            KillProcess();
        }

        #endregion

        internal async Task CreateTask(CancellationToken cancel)
        {
            Task.PreExecution(Options.Backup);
            TaskJournal = CreateTaskJournal();
            ReportStatus(TaskAppStatus.Started);
            using (var taskCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel))
            {
                taskCancel.Token.Register(CancelProcess);
                using (Process = new RevitProcess(TaskArguments))
                {
                    var normalExit = await Process.RunTaskAsync(TaskJournal, taskCancel.Token)
                                                  .ConfigureAwait(false);
                    if (normalExit == false)
                    {
                        ReportStatus(TaskAppStatus.Timeout);
                    }
                }
            }
            Process = null;
            ReportStatus(TaskAppStatus.Finish);
        }

        private RevitArguments TaskArguments
        {
            get
            {
                if (Options.UseMetadata && TaskManager.IsRevitInstalled(Task.Family, out var revitApp))
                {
                    var timeout = Options.Arguments.Timeout;
                    return new RevitArguments
                    {
                        Timeout = timeout,
                        RevitApp = revitApp
                    };
                }
                return Options.Arguments;
            }
        }

        public void ReportStatus(int status)
        {
            Status.SetStatus(status);
            Progress?.Report(this);
        }

        public void Cleanup()
        {
            Process?.KillProcess();
            DisconnectAction?.Invoke(TaskId);
            ReportManager.CreateLogs();
            Task.DeleteBackups(Options);
            Progress = null;
        }

        public Action<string> DisconnectAction { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as TaskUnitOfWork);
        }

        public bool Equals(TaskUnitOfWork other)
        {
            return other != null &&
                   EqualityComparer<RevitTask>.Default.Equals(Task, other.Task);
        }

        public override int GetHashCode()
        {
            return 658265964 + EqualityComparer<RevitTask>.Default.GetHashCode(Task);
        }

        public static bool operator ==(TaskUnitOfWork left, TaskUnitOfWork right)
        {
            return EqualityComparer<TaskUnitOfWork>.Default.Equals(left, right);
        }

        public static bool operator !=(TaskUnitOfWork left, TaskUnitOfWork right)
        {
            return !(left == right);
        }
    }
}
