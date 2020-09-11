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
        private static readonly ActionManager actionManager = new ActionManager();

        private static readonly NullRecordeJournalFile nullRecorde = new NullRecordeJournalFile();
        private static readonly NullTaskJournalFile nullTask = new NullTaskJournalFile();
        private static readonly ITaskAction nullAction = new NullTaskAction();

        public RevitTask Task { get; private set; }

        public TaskOptions Options { get; private set; }

        public TaskAppStatus Status { get; private set; } = new TaskAppStatus();

        public TaskJournalFile TaskJournal { get; set; } = nullTask;

        public RecordeJournalFile RecordeJournal { get; set; } = nullRecorde;

        //public TaskReport Report { get; set; } = new TaskReport();

        public RevitProcess Process { get; set; }

        public IDictionary<ITaskAction, TaskActionReport> ActionReports { get; }
            = new Dictionary<ITaskAction, TaskActionReport>();

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
                nextActionId = ActionManager.JournalActionId;
            }
            else
            {
                if (actionManager.IsJournalAction(actionId))
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
            switch (report.Kind)
            {
                case ReportKind.Status:
                    if (actionManager.IsOpenAction(report.ActionId)
                        || actionManager.IsSaveAction(report.ActionId))
                    {
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
                    }
                    break;
                case ReportKind.Error:
                    if (ErrorAction is null && Task.HasActionById(report.ActionId, out var errorAction))
                    {
                        ErrorAction = errorAction;
                        var message = report.Message;
                        if (report.Exception is object)
                        {
                            message += Environment.NewLine + "Exception Message: " + report.Exception.Message;
                            message += Environment.NewLine + "Exception StackTrace: " + report.Exception.StackTrace;
                        }
                        ErrorMessage = message;
                    }
                    break;
                case ReportKind.Warning:
                    if (HasActionReport(report.ActionId, out var warningReport))
                    {
                        warningReport.Add(report);
                    }
                    break;
                default:
                    break;
            }

            Status.SetStatus(GetTaskStatus(report));
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
                case ReportKind.Warning:
                default:
                    break;
            }
            return status;
        }

        private TFile CreateFile<TFile>(ReportMessage report) where TFile : AFile, new()
        {
            return new TFile { FullPath = report.Message };
        }

        public void DeleteBackups()
        {
            if (Options.DeleteRevitBackup == false) { return; }

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
            if (Task.HasActionById(actionId, out var action))
            {
                if (ActionReports.ContainsKey(action) == false)
                {
                    ActionReports.Add(action, new TaskActionReport { TaskAction = action });
                }
                actionReport = ActionReports[action];
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

            Task.PreExecution(Options.Backup);
            TaskJournal = CreateTaskJournal();
            ReportStatus(progress, TaskAppStatus.Started);
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
                ReportStatus(progress, TaskAppStatus.Finish);
            }
            ReportStatus(progress, TaskAppStatus.CleanUp);
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

        public void ReportStatus(IProgress<TaskUnitOfWork> progress, int status)
        {
            if (progress is null) { return; }

            Status.SetStatus(status);
            progress.Report(this);
        }

        public void Cleanup()
        {
            Process.KillProcess();
            DisconnectAction?.Invoke(TaskId);
            DeleteBackups();
            CreateLogs();
        }

        private void CreateLogs()
        {
            var result = new TaskReport
            {
                SourceFile = Task.SourceFile,
                ResultFile = Task.ResultFile,
                BackupFile = Task.BackupFile,
                TaskJournal = TaskJournal,
                RecordeJournal = RecordeJournal
            };
            if (Options.Report.LogError && ErrorAction is object)
            {
                result.ErrorReport = ErrorAction;
                result.ErrorMessage = ErrorMessage;
            }

            if (Options.Report.LogSuccess || Options.Report.LogResults)
            {
                foreach (var action in Task.Actions)
                {
                    if (ActionReports.ContainsKey(action) == false) { continue; }

                    if (ActionReports[action].HasStatusReports())
                    {
                        result.SuccessReport.AddRange(GetStatus(action));
                    }
                    if (ActionReports[action].HasWarningReports())
                    {
                        result.WarningReport.AddRange(GetWarnings(action));
                    }
                }
            }
            TaskReport.Write(result);
        }

        private IEnumerable<string> GetWarnings(ITaskAction action)
        {
            return GetMessages(action, ActionReports[action].WarningReports(), "WARNING");
        }

        private IEnumerable<string> GetStatus(ITaskAction action)
        {
            return GetMessages(action, ActionReports[action].StatusReports(), "STATUS");
        }

        private IEnumerable<string> GetMessages(ITaskAction action, IEnumerable<string> messages, string type)
        {
            var builder = new List<string>();
            foreach (var message in messages)
            {
                var line = $"{action.Name}: {type}: {message}";
                builder.Add(line);
            }
            return builder;
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
