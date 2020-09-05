using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitAction.Report;
using RevitAction.Report.Message;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
using RevitJournal.Tasks.Actions;
using RevitJournal.Tasks.Options;
using RevitJournal.Tasks.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Tasks
{
    public class TaskUnitOfWork : IReportReceiver
    {
        private static readonly NullRecordeJournalFile nullRecorde = new NullRecordeJournalFile();
        private static readonly NullTaskJournalFile nullTask = new NullTaskJournalFile();
        private static readonly ITaskAction nullAction = new NullTaskAction();

        public RevitTask Task { get; private set; }

        public TaskOptions Options { get; private set; }

        public ReportStatus Status { get; private set; } = new ReportStatus();

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

        public Progress<TaskUnitOfWork> Progress { get; set; } = new Progress<TaskUnitOfWork>();

        public IProgress<TaskUnitOfWork> ProgressReport
        {
            get { return Progress; }
        }

        public IDictionary<Guid, TaskActionReport> ActionReports { get; } = new Dictionary<Guid, TaskActionReport>();

        public TaskUnitOfWork(RevitTask task, TaskOptions options)
        {
            Task = task;
            Options = options;
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

        public int ExecutedActions { get; private set; }

        public void SetStatus(int status)
        {
            Status.SetStatus(status);
            ProgressReport.Report(this);
        }

        public void MakeReport(ReportMessage report)
        {
            if (report is null) { return; }

            var actionReport = GetActionReport(report.ActionId);
            switch (report.Kind)
            {
                case ReportKind.Open:
                    var openFile = CreateFile<RevitFamilyFile>(report);
                    Task.ResultFile = openFile;
                    if (Task.SourceFile.Equals(openFile) == false)
                    {
                        var message = "Source and opened file are not equals";
                        Debug.WriteLine(message);
                    }
                    break;
                case ReportKind.Journal:
                    var journalFile = CreateFile<RecordeJournalFile>(report);
                    RecordeJournal = journalFile;
                    break;
                case ReportKind.Status:
                    var actionStatus = ActionStatusUtil.ToEnum(report);
                    actionReport.Status = actionStatus;
                    if (actionReport.IsExecuted)
                    {
                        ExecutedActions += 1;
                    }
                    break;
                case ReportKind.Success:
                    actionReport.Add(report);
                    break;
                case ReportKind.Error:
                    actionReport.Add(report);
                    KillProcess();
                    break;
                case ReportKind.Save:
                case ReportKind.SaveAs:
                    var saveFile = CreateFile<RevitFamilyFile>(report);
                    Task.ResultFile = saveFile;
                    break;
                case ReportKind.Close:
                    KillProcess();
                    DeleteBackups(Task);
                    break;
                case ReportKind.Unknown:
                default:
                    break;
            }

            Status.SetStatus(report.GetTaskStatus());
            CurrentAction = GetCurrentAction(report);
            ProgressReport.Report(this);
        }

        private ITaskAction GetCurrentAction(ReportMessage report)
        {
            if (CurrentAction.Id == report.ActionId || report.IsFinished)
            {
                return CurrentAction;
            }
            else if (Task.HasActionById(report.ActionId, out var action))
            {
                return action;
            }
            return nullAction;
        }

        private TFile CreateFile<TFile>(ReportMessage report) where TFile : AFile, new()
        {
            return new TFile { FullPath = report.Message };
        }

        private void DeleteBackups(RevitTask task)
        {
            if (Options.DeleteRevitBackup == false) { return; }

            DeleteBackups(task.SourceFile);
            if (task.HasResultFile)
            {
                DeleteBackups(task.ResultFile);
            }

            if (task.HasBackupFile)
            {
                DeleteBackups(task.BackupFile);
            }
        }

        private void DeleteBackups(RevitFamilyFile familyFile)
        {
            foreach (var file in familyFile.Backups)
            {
                file.Delete();
            }
        }

        private TaskActionReport GetActionReport(Guid actionId)
        {
            if (ActionReports.ContainsKey(actionId) == false)
            {
                ActionReports.Add(actionId, new TaskActionReport());
            }
            return ActionReports[actionId];
        }

        #region Process

        public void CancelProcess()
        {
            KillProcess();
            SetStatus(ReportStatus.Cancel);
        }

        public void KillProcess()
        {
            if (Process is null) { return; }

            Process.KillProcess();
        }

        #endregion

        internal async Task CreateTask(CancellationToken cancel)
        {
            SetStatus(ReportStatus.Started);
            PreExecution();
            TaskJournal = CreateTaskJournal();
            using (var taskCancel = CancellationTokenSource.CreateLinkedTokenSource(cancel))
            {
                taskCancel.Token.Register(CancelProcess);
                using (Process = new RevitProcess(Options.Arguments))
                {
                    var normalExit = await Process.RunTaskAsync(TaskJournal, taskCancel.Token)
                                                  .ConfigureAwait(false);
                    if (normalExit == false)
                    {
                        KillProcess();
                        SetStatus(ReportStatus.Timeout);
                    }
                }
            }
            SetStatus(ReportStatus.Finish);
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
    }
}
