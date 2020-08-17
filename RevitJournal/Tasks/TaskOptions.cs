using DataSource.Model.FileSystem;
using RevitJournal.Tasks.Options;
using System;
using Utilities;

namespace RevitJournal.Tasks
{
    public class TaskOptions
    {
        private static string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public ReportOptions Report { get; set; } = new ReportOptions();

        public BackupOptions Backup { get; set; } = new BackupOptions();

        public ParallelOptions Parallel { get; set; } = new ParallelOptions();

        public RevitArguments Arguments { get; set; } = new RevitArguments();

        public RevitDirectory RootDirectory { get; private set; } = new RevitDirectory(null, MyDocuments);

        public string JournalDirectory { get; set; } = MyDocuments;
        
        public string ActionDirectory { get; set; } = MyDocuments;

        public bool DeleteRevitBackup { get; set; } = true;

        public void SetRootDirectory(string directory)
        {
            if (RootDirectory is null ||
               StringUtils.Equals(RootDirectory.FullPath, directory) == false)
            {
                RootDirectory = new RevitDirectory(null, directory);
            }
        }

        public bool IsRootDirectory(string directory)
        {
            return RootDirectory != null
                && StringUtils.Equals(RootDirectory.FullPath, directory);
        }
    }
}
