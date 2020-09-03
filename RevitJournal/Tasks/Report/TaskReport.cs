using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using DataSource.Model.FileSystem;
using RevitAction.Report;

namespace RevitJournal.Tasks.Report
{
    public class TaskReport : ITaskReport
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

        private RevitTask Task { get; set; }

        public RevitFamilyFile SourceFile
        {
            get { return Task.Family.RevitFile; }
        }

        public RevitFamilyFile ResultFile { get; set; }

        public RevitFamilyFile BackupFile { get; set; }

        public TaskReport(RevitTask task)
        {
            Task = task ?? throw new ArgumentNullException(nameof(task));
            ResultFile = Task.SourceFile;
        }

        public bool IsTask(RevitTask revitTask)
        {
            return Task.Equals(revitTask);
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
