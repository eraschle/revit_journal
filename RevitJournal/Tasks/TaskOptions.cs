using RevitJournal.Tasks.Options;
using System;

namespace RevitJournal.Tasks
{
    public class TaskOptions
    {
        private static string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public ReportOptions Report { get; set; } = new ReportOptions();

        public BackupOptions Backup { get; set; } = new BackupOptions();

        public ParallelOptions Parallel { get; set; } = new ParallelOptions();

        public RevitArguments Arguments { get; set; } = new RevitArguments();

        public string RootDirectory { get; set; } = MyDocuments;

        public string JournalDirectory { get; set; } = MyDocuments;
        
        public string ActionDirectory { get; set; } = MyDocuments;

        public bool DeleteRevitBackup { get; set; } = true;
    }
}
