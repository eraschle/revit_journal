using DataSource.Model.FileSystem;
using DataSource.Model.Product;
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
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Tasks
{
    public class TaskUnitOfWork : IReportReceiver, IEquatable<TaskUnitOfWork>
    {
        private static readonly ActionManager actionManager = new ActionManager();

        private static readonly NullRecordeJournalFile nullRecorde = new NullRecordeJournalFile();
        private static readonly NullTaskJournalFile nullTask = new NullTaskJournalFile();
        private static readonly ITaskAction nullAction = new NullTaskAction();

        public RevitTask Task { get; private set; }

        public TaskOptions Options { get; private set; }

        public TaskAppStatus Status { get; private set; } = new TaskAppStatus();

        public TaskJournalFile TaskJournal { get; set; } = nullTask;

        public bool HasTaskJournal
        {
            get { return TaskJournal != nullTask; }
        }

        public RecordeJournalFile RecordeJournal { get; set; } = nullRecorde;

        public bool HasRecordeJournal
        {
            get { return RecordeJournal != nullRecorde; }
        }

        //public TaskReport Report { get; set; } = new TaskReport();

        public RevitProcess Process { get; set; }

        public IDictionary<Guid, TaskActionReport> ActionReports { get; } = new Dictionary<Guid, TaskActionReport>();

        public ITaskAction ErrorAction { get; private set; } = null;

        public string ErrorMessage { get; private set; } = null;

        public TaskUnitOfWork(RevitTask task, TaskOptions options)
        {
            Task = task;
            Options = options;
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
            if (actionManager.IsOpenAction(actionId))
            {
                nextActionId = actionManager.JournalActionId;
            }
            else if (actionManager.IsSaveAction(actionId)
                || actionManager.IsCloseAction(actionId))
            {
                nextActionId = actionManager.CloseActionId;
            }
            else
            {
                if (actionManager.IsJournalAction(actionId))
                {
                    actionId = actionManager.OpenActionId;
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

            switch (report.Kind)
            {
                case ReportKind.Status:
                    if (actionManager.IsOpenAction(report.ActionId)
                        || actionManager.IsSaveAction(report.ActionId))
                    {
                        SetNewAction(report);
                        var resultFile = CreateFile<RevitFamilyFile>(report);
                        Task.ResultFile = resultFile;
                    }
                    else if (actionManager.IsJournalAction(report.ActionId))
                    {
                        var journalFile = CreateFile<RecordeJournalFile>(report);
                        RecordeJournal = journalFile;
                    }
                    if (HasActionReport(report.ActionId, out var statusReport))
                    {
                        statusReport.Add(report);
                        Debug.WriteLine($"Report: {report}");
                    }
                    break;
                case ReportKind.Error:
                    if (ErrorAction is null && Task.HasActionById(report.ActionId, out var errorAction))
                    {
                        ErrorAction = errorAction;
                        ErrorMessage = report.Message;
                    }
                    KillProcess();
                    break;
                default:
                    break;
            }

            Status.SetStatus(GetTaskStatus(report));
        }

        private void SetNewAction(ReportMessage report)
        {
            if (Task.HasActionById(report.ActionId, out var action))
            {
                ExecutedActions += 1;
                CurrentAction = action;
                Debug.WriteLine($"Set current action {action} / executed {ExecutedActions}");
            }
        }

        private int GetTaskStatus(ReportMessage report)
        {
            var status = TaskAppStatus.Run;
            switch (report.Kind)
            {
                case ReportKind.Status:
                    if (actionManager.IsOpenAction(report.ActionId))
                    {
                        status = TaskAppStatus.Open;
                    }
                    break;
                case ReportKind.Error:
                    status = TaskAppStatus.Error;
                    break;
                default:
                    break;
            }
            Debug.WriteLine($"Task status: {status}");
            return status;
        }

        private TFile CreateFile<TFile>(ReportMessage report) where TFile : AFile, new()
        {
            return new TFile { FullPath = report.Message };
        }

        public void DeleteBackups()
        {
            if (Options.DeleteRevitBackup == false) { return; }

            Debug.WriteLine($"Delete backups");
            Task.SourceFile.DeleteBackups();
            if (Task.HasResultFile)
            {
                Task.ResultFile.DeleteBackups();
            }

            if (Task.HasBackupFile)
            {
                Task.BackupFile.DeleteBackups();
            }
        }

        private bool HasActionReport(Guid actionId, out TaskActionReport actionReport)
        {
            actionReport = null;
            if (actionManager.IsAppAction(actionId) == false)
            {
                if (ActionReports.ContainsKey(actionId) == false)
                {
                    ActionReports.Add(actionId, new TaskActionReport());
                }
                actionReport = ActionReports[actionId];
            }
            return actionReport != null;
        }

        #region Process

        public void KillProcess()
        {
            if (Process is null) { return; }

            Process.KillProcess();
        }

        #endregion

        internal async Task CreateTask(IProgress<TaskUnitOfWork> progress, CancellationToken cancel)
        {
            if (progress is null) { throw new ArgumentNullException(nameof(progress)); }
            if (TaskArguments is null) { throw new ArgumentNullException(nameof(TaskArguments)); }

            PreExecution();
            TaskJournal = CreateTaskJournal();
            Status.SetStatus(TaskAppStatus.Started);
            using (var taskCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel))
            {
                taskCancel.Token.Register(KillProcess);
                using (Process = new RevitProcess(TaskArguments))
                {
                    var normalExit = await Process.RunTaskAsync(TaskJournal, taskCancel.Token)
                                                  .ConfigureAwait(false);
                    if (normalExit == false)
                    {
                        ReportStatus(progress, TaskAppStatus.Timeout);
                    }
                }
                if (taskCancel.IsCancellationRequested)
                {
                    ReportStatus(progress, TaskAppStatus.Cancel);
                }
            }
            ReportStatus(progress, TaskAppStatus.Finish);
            ReportStatus(progress, TaskAppStatus.CleanUp);
        }

        private RevitArguments TaskArguments
        {
            get
            {
                if (Options.UseMetadata == false)
                {
                    return Options.Arguments;
                }
                if (TaskManager.IsRevitInstalled(Task.Family, out var revitApp))
                {
                    var timeout = Options.Arguments.Timeout;
                    return new RevitArguments
                    {
                        Timeout = timeout,
                        RevitApp = revitApp
                    };
                }
                return null;
            }
        }

        private void ReportStatus(IProgress<TaskUnitOfWork> progress, int status)
        {
            Status.SetStatus(status);
            progress.Report(this);
        }

        private void PreExecution()
        {
            if (Options.Backup.CreateBackup)
            {
                var backupPath = Options.Backup.CreateBackupFile(Task.SourceFile);
                Task.BackupFile = Task.SourceFile.CopyTo<RevitFamilyFile>(backupPath, true);
            }

            foreach (var command in Task.Actions)
            {
                command.PreTask(Task.Family);
            }
        }

        public void Cleanup()
        {
            DisconnectAction?.Invoke(TaskId);
            DeleteBackups();
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
