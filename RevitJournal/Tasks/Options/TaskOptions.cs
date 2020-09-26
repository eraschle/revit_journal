using DataSource.Model.FileSystem;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
using System;
using Utilities.System;

namespace RevitJournal.Tasks.Options
{
    public class TaskOptions
    {
        private static readonly string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        private static string[] formats = new string[]
        {
            DateUtils.YearLong,
            DateUtils.MonthShort,
            DateUtils.Day,
            DateUtils.Hour,
            DateUtils.Minute 
        };

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

        public DateTime JournalTimeDirectory { get; set; }

        public DirectoryNode GetJournalWorking(IPathBuilder builder)
        {
            if(builder is null) { throw new ArgumentNullException(nameof(builder)); }

            var directory = builder.CreateRoot(JournalDirectory);
            var timeFolder = GetTimeFolder(builder, directory);
            if (timeFolder.Exists() == false)
            {
                timeFolder.Create();
            }
            return timeFolder;
        }

        private DirectoryNode GetTimeFolder(IPathBuilder builder, DirectoryNode directory)
        {
            var journalTime = JournalTimeDirectory;
            var timeFolderName = DateUtils.AsString(journalTime, Constant.Minus, formats);
            return builder.InsertFolder(directory, timeFolderName);
        }

        public string ActionDirectory { get; set; } = MyDocuments;

        public bool DeleteRevitBackup { get; set; } = true;
    }
}
