using DataSource.Model.FileSystem;
using RevitAction.Report;
using RevitAction.Report.Message;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
using RevitJournal.Tasks.Options;
using RevitJournal.Tasks.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Tasks
{
    public class TaskUnitOfWork : IReportReceiver
    {
        public RevitTask Task { get; private set; }

        public TaskOptions Options { get; private set; }

        public ReportStatus Status { get; private set; }

        public TaskJournalFile TaskJournal { get; set; }

        public bool HasTaskJournal
        {
            get { return TaskJournal != null; }
        }

        public RecordeJournalFile RecordeJournal { get; set; }

        public bool HasRecordeJournal
        {
            get { return RecordeJournal != null; }
        }

        public TaskReport Report { get; set; } = new TaskReport();

        public RevitProcess Process { get; set; }

        public Progress<TaskUnitOfWork> Progress { get; set; } = new Progress<TaskUnitOfWork>();

        public IProgress<TaskUnitOfWork> ProgressReport
        {
            get { return Progress; }
        }

        public Guid CurrentActionId { get; set; } = Guid.Empty;

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

        public void SetStatus(int status)
        {
            SetStatus(status, true);
        }

        private void SetStatus(int status, bool doReport)
        {
            Status.SetStatus(status);
            if (doReport == false) { return; }

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
                    if(Task.SourceFile.Equals(openFile) == false)
                    {
                        var message = "Source and opened file are not equals";
                        throw new ArgumentException(message);
                    }
                    Report.SourceFile = openFile;
                    Report.ResultFile = openFile;
                    break;
                case ReportKind.Journal:
                    RecordeJournal = CreateFile<RecordeJournalFile>(report);
                    break;
                case ReportKind.Status:
                    actionReport.Status = report.Message;
                    break;
                case ReportKind.Error:
                case ReportKind.Success:
                    actionReport.Add(report);
                    break;
                case ReportKind.Save:
                case ReportKind.SaveAs:
                    Report.ResultFile = CreateFile<RevitFamilyFile>(report);
                    break;
                case ReportKind.Close:
                    if (Options.DeleteRevitBackup)
                    {
                        DeleteBackups(Task.SourceFile);
                        DeleteBackups(Report.ResultFile);
                    }
                    break;
                case ReportKind.Unknown:
                default:
                    break;
            }
            if (report.IsError)
            {
                KillProcess();
            }

            SetStatus(report.GetStatus(), false);
            CurrentActionId = report.ActionId;
            ProgressReport.Report(this);
        }

        private TFile CreateFile<TFile>(ReportMessage report) where TFile : AFile, new()
        {
            return new TFile { FullPath = report.Message };
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
            Process.Dispose();
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
            var option = Options.Backup;
            if (option.CreateBackup)
            {
                var backupPath = option.CreateBackupFile(Task.SourceFile);
                Report.BackupFile = Task.SourceFile.CopyTo<RevitFamilyFile>(backupPath, true);
            }

            foreach (var command in Task.Actions)
            {
                command.PreTask(Task.Family);
            }
        }
    }
}
