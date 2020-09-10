﻿using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using DataSource.Model.FileSystem;
using RevitAction.Report;
using RevitJournal.Revit.Journal;
using RevitAction.Action;

namespace RevitJournal.Report
{
    public class TaskReport : ITaskReport
    {
        public static void Write(TaskReport result)
        {
            if (result is null) { return; }

            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var content = JsonConvert.SerializeObject(result, Formatting.Indented, setting);
            var resultFile = GetResultFile(result);
            File.WriteAllText(resultFile.FullPath, content);
        }

        private static TaskReportFile GetResultFile(TaskReport result)
        {
            return result.SourceFile
                .ChangeExtension<TaskReportFile>(TaskReportFile.TaskResultExtension)
                .ChangeFileName<TaskReportFile>(result.SourceFile.Name + "_Result");
        }

        public RevitFamilyFile SourceFile { get; set; }

        public TaskJournalFile TaskJournal { get; set; }

        public RecordeJournalFile RecordeJournal { get; set; }

        public RevitFamilyFile ResultFile { get; set; }

        public RevitFamilyFile BackupFile { get; set; }

        public List<string> SuccessReport { get; } = new List<string>();

        public List<string> WarningReport { get; } = new List<string>();

        public ITaskAction ErrorReport { get; set; } = null;

        public string ErrorMessage { get; set; } = null;

        public override bool Equals(object obj)
        {
            return obj is TaskReport result &&
                   EqualityComparer<RevitFamilyFile>.Default.Equals(SourceFile, result.SourceFile);
        }

        public override int GetHashCode()
        {
            return -174109371 + EqualityComparer<RevitFamilyFile>.Default.GetHashCode(SourceFile);
        }
    }
}