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
using SYS = System.Threading.Tasks;
using Utilities.System;
using System.Diagnostics;

namespace RevitJournal.Tasks
{
    public class TaskUnitOfWork : IReportReceiver, IEquatable<TaskUnitOfWork>
    {
        private static readonly RecordJournalNullFile nullRecorde = new RecordJournalNullFile();
        private static readonly TaskJournalNullFile nullTask = new TaskJournalNullFile();
        private static readonly ITaskAction nullAction = new NullTaskAction();

        public RevitTask Task { get; private set; }

        public TaskOptions Options { get; private set; }

        public TaskAppStatus Status { get; private set; } = new TaskAppStatus();

        public TaskJournalFile TaskJournal { get; set; } = nullTask;

        public RecordJournalFile RecordeJournal { get; set; } = nullRecorde;

        public TaskReportManager ReportManager { get; private set; }

        public RevitProcess Process { get; set; }

        public IProgress<TaskUnitOfWork> Progress { get; set; }

        public Action<string> DisconnectAction { get; set; }

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
            var dataSource = new TaskJournalDataSource();
            var workingDirectory = Options.GetJournalWorking(PathFactory.Instance);
            var journalFile = Task.SourceFile.ChangeDirectory<TaskJournalFile>(workingDirectory);
            dataSource.SetFile(journalFile);
            dataSource.Write(Task);
            return dataSource.FileNode;
        }

        public void DeleteJournalProcess()
        {
            if (TaskJournal is null) { return; }

            TaskJournal.Delete();
        }

        public ITaskAction CurrentAction { get; private set; } = nullAction;

        public int ExecutedActions { get; private set; } = 0;

        public void MakeReport(ReportMessage report)
        {
            if (report is null) { return; }

            var factory = PathFactory.Instance;
            SetNewAction(report);
            Status.SetStatus(GetTaskStatus(report));
            ReportManager.AddReport(report);
            if (ActionManager.IsOpenAction(report.ActionId)
                || ActionManager.IsSaveAction(report.ActionId))
            {
                var resultFile = factory.Create<RevitFamilyFile>(report.Message);
                Task.ResultFile = resultFile;
            }
            else if (ActionManager.IsJournalAction(report.ActionId))
            {
                var journalFile = factory.Create<RecordJournalFile>(report.Message);
                var journalRoot = Options.GetJournalWorking(factory);
                var search = $"{journalFile.NameWithoutExtension}{Constant.Star}";
                factory.CreateFiles<RecordJournalFile>(journalRoot, search);
                RecordeJournal = journalFile;
            }
            Progress?.Report(this);
        }

        private void SetNewAction(ReportMessage report)
        {
            if (CurrentAction.ActionId.Equals(report.ActionId) == false
                && Task.HasActionById(report.ActionId, out var action))
            {
                ExecutedActions += 1;
                CurrentAction = action;
            }
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
            DisconnectAction?.Invoke(TaskId);
            KillProcess();
        }

        #endregion

        internal async SYS.Task CreateTask(CancellationToken cancel)
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
                Process = null;
            }
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
            Task.DeleteBackups(Options);
            DisconnectAction?.Invoke(TaskId);
            Progress = null;

            if (DoesRecordCopyExists(out var copy)) { return; }

            if (ReportManager.HasErrorReport())
            {
                ReportManager.CreateErrorReport(copy, "ERROR");
            }
            if (ReportManager.HasSuccessReport())
            {
                ReportManager.CreateSuccessReport(copy, "Success");
            }
            CopyRecordJournal();

        }

        private bool HasRecordJournal()
        {
            return RecordeJournal is object && RecordeJournal.Exists();
        }

        public RecordJournalFile GetRenamedJournalFile()
        {
            if (HasRecordJournal() == false 
                || Task.SourceFile is null 
                || Task.SourceFile.HasParent(out var directory) == false) { return null; }

            var newFileName = $"{Task.Name}_{RecordeJournal.NameWithoutExtension}";
            return RecordeJournal.ChangeFileName<RecordJournalFile>(newFileName)
                                 .ChangeDirectory<RecordJournalFile>(directory);
        }

        public bool DoesRecordCopyExists(out RecordJournalFile copied)
        {
            copied = GetRenamedJournalFile();
            return copied is object && copied.Exists();
        }

        private async void CopyRecordJournal()
        {
            await SYS.Task.Run(() =>
            {
                while (HasRecordJournal() && DoesRecordCopyExists(out var copy) == false)
                {
                    try
                    {
                        RecordeJournal.CopyTo(copy, overrideFile: true);
                        Debug.WriteLine($"Copy Name: {copy.Name} [{copy.FullPath}]");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Wait: {copy.Name} Exception {ex.Message}");
                        SYS.Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
                    }
                }
            }).ConfigureAwait(false);
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
