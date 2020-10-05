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
using Utilities.System;

namespace RevitJournal.Tasks
{
    public class TaskUnitOfWork : IReportReceiver, IEquatable<TaskUnitOfWork>
    {
        private static readonly ITaskAction nullAction = new NullTaskAction();

        public RevitTask Task { get; private set; }

        public ITaskAction CurrentAction { get; private set; } = nullAction;

        public TaskOptions Options { get; private set; }

        public TaskAppStatus Status { get; private set; } = new TaskAppStatus();

        public TaskReportManager ReportManager { get; private set; } = new TaskReportManager();

        public RevitProcess Process { get; set; }

        public IProgress<TaskUnitOfWork> Progress { get; set; }

        public Action<string> DisconnectAction { get; set; }

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
            try
            {
                Task.CreateTaskJournal(Options);
                var dataSource = new TaskJournalDataSource();
                dataSource.SetFile(Task.TaskJournal);
                dataSource.Write(Task);
            }
            catch (Exception)
            {
                Task.SetDefaultTaskJournal();
                throw;
            }
            return Task.TaskJournal;
        }

        public void DeleteJournalProcess()
        {
            if (Task.TaskJournal is null) { return; }

            Task.TaskJournal.Delete();
        }

        public int ExecutedActions
        {
            get
            {
                var count = 0;
                if (CurrentAction.Equals(nullAction) == false)
                {
                    count = Task.GetExecutedActions(CurrentAction);
                }
                return count;
            }
        }

        public void MakeReport(ReportMessage report)
        {
            if (report is null) { return; }

            var factory = PathFactory.Instance;
            SetNewAction(report);
            Status.SetStatus(GetTaskStatus(report));
            ReportManager.AddReport(Task, report);
            if (ActionManager.IsOpenAction(report.ActionId)
                || ActionManager.IsSaveAction(report.ActionId))
            {
                var resultFile = factory.Create<RevitFamilyFile>(report.Message);
                Task.ResultFile = resultFile;
            }
            else if (ActionManager.IsJournalAction(report.ActionId))
            {
                var journalFile = factory.Create<RecordJournalFile>(report.Message);
                Task.RecordeJournal = journalFile;
            }
            Progress?.Report(this);
        }

        private void SetNewAction(ReportMessage report)
        {
            if (CurrentAction.ActionId.Equals(report.ActionId)
                || Task.HasActionById(report.ActionId, out var action) == false)
            {
                return;
            }

            CurrentAction = action;
        }

        private int GetTaskStatus(ReportMessage report)
        {
            switch (report.Kind)
            {
                case ReportKind.Message:
                case ReportKind.Warning:
                    return TaskAppStatus.Running;
                case ReportKind.Error:
                    return TaskAppStatus.Error;
                default:
                    return TaskAppStatus.Started;
            }
        }

        #region Process

        public void KillProcess()
        {
            try
            {
                Process.KillProcess();
            }
            finally
            {
                Process.Dispose();
            }
        }

        public void CancelProcess()
        {
            ReportStatus(TaskAppStatus.Cancel);
            KillProcess();
        }

        #endregion

        internal async System.Threading.Tasks.Task CreateTask(CancellationToken cancel)
        {
            Task.PreExecution(Options);
            var journal = CreateTaskJournal();
            ReportStatus(TaskAppStatus.Started);
            using (var taskCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel))
            {
                taskCancel.Token.Register(CancelProcess);
                Process = new RevitProcess(TaskArguments);
                Process.ProcessFinished += Process_ProcessFinished;
                var normalExit = await Process.RunTaskAsync(journal, taskCancel.Token)
                                              .ConfigureAwait(false);
                if (normalExit == false)
                {
                    ReportStatus(TaskAppStatus.Timeout);
                }
            }
            Process.WaitChildProcessesExited();
            ReportStatus(TaskAppStatus.Finish);
        }

        private RevitArguments TaskArguments
        {
            get
            {
                if (Options.UseMetadata && TaskManager.IsRevitInstalled(Task.Family, out var revitApp))
                {
                    var timeout = Options.ProcessTimeout;
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

        private void Process_ProcessFinished(object sender, EventArgs e)
        {
            Process.ProcessFinished -= Process_ProcessFinished;
            Process = null;
            ReportStatus(TaskAppStatus.CleanUp);
            DisconnectAction?.Invoke(TaskId);

            if (Task.DoesRecordCopyExists()) { return; }

            if (ReportManager.HasErrorReport(Options))
            {
                ReportManager.CreateErrorReport(Task, Options, "ERROR");
            }
            if (ReportManager.HasSuccessReport(Task, Options))
            {
                ReportManager.CreateSuccessReport(Task, Options, "Success");
            }
            Task.CopyRecordJournal();
            Task.DeleteBackups(Options);
        }

        public string GetTaskJournalName()
        {
            return Task.TaskJournal.NameWithoutExtension;
        }

        public string GetRecordJournalName()
        {
            return Task.RecordeJournal.NameWithoutExtension;
        }

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
