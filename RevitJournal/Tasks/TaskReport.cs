using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using RevitJournal.Journal.Execution;
using DataSource.Model.FileSystem;
using RevitJournal.Journal;
using RevitAction.Reports;

namespace RevitJournal.Tasks
{
    public class TaskReport : IReportReceiver
    {
        public static void Write(TaskReport result)
        {
            if (result is null) { return; }

            var content = JsonConvert.SerializeObject(result, Formatting.Indented);
            var resultFile = GetResultFile(result);
            File.WriteAllText(resultFile.FullPath, content);
        }

        private static TaskReportFile GetResultFile(TaskReport result)
        {
            return result.SourceFile
                .ChangeExtension<TaskReportFile>(TaskReportFile.TaskResultExtension)
                .ChangeFileName<TaskReportFile>(result.SourceFile.Name + "_Result");
        }

        public string GetId()
        {
            return SourceFile.FullPath;
        }

        private RevitTask Task { get; set; }

        public TaskJournalFile TaskJournal
        {
            get { return Task.JournalTask; }
        }

        public bool HasTaskJournal
        {
            get { return TaskJournal != null; }
        }

        public RecordeJournalFile RecordeJournal { get; set; }

        public bool HasRecordeJournal
        {
            get { return RecordeJournal != null; }
        }

        public RevitFamilyFile SourceFile
        {
            get { return Task.Family.RevitFile; }
        }

        public RevitFamilyFile ResultFile { get; internal set; }

        public RevitFamilyFile BackupFile { get; internal set; }

        public TaskReportStatus Status { get; private set; }

        public TaskReport(RevitTask task)
        {
            Task = task ?? throw new ArgumentNullException(nameof(task));
            Status = new TaskReportStatus();
        }

        public bool IsTask(RevitTask revitTask)
        {
            return Task.Equals(revitTask);
        }

        public void SetReport(int status)
        {
            if (TaskReportStatus.IsStatus(status) == false) { return; }

            Status.SetStatus(status);
        }

        public void MakeReport(ReportData report)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return obj is TaskReport result &&
                   EqualityComparer<RevitTask>.Default.Equals(Task, result.Task);
        }

        public override int GetHashCode()
        {
            return -174109371 + EqualityComparer<RevitTask>.Default.GetHashCode(Task);
        }
    }
}
