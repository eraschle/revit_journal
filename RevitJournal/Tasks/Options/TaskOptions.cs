using RevitJournal.Revit;
using System;

namespace RevitJournal.Tasks.Options
{
    public class TaskOptions
    {
        private static readonly string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public ReportOptions Report { get; set; } = new ReportOptions();

        public BackupOptions Backup { get; set; } = new BackupOptions();

        public ParallelOptions Parallel { get; set; } = new ParallelOptions();

        public RevitArguments Arguments { get; set; } = new RevitArguments();

        public bool UseMetadata
        {
            get { return Arguments.RevitApp.UseMetadata; }
        }

        public TimeSpan Timeout
        {
            get { return Arguments.Timeout; }
        }

        public string RootDirectory { get; set; } = MyDocuments;

        public string JournalDirectory { get; set; } = MyDocuments;

        public string ActionDirectory { get; set; } = MyDocuments;

        public bool DeleteRevitBackup { get; set; } = true;
    }
}
