using DataSource.Model.FileSystem;
using RevitJournal.Helper;
using RevitJournal.Revit;
using System;
using Utilities.System;
using Utilities.System.FileSystem;

namespace RevitJournal.Tasks.Options
{
    public class TaskOptions
    {
        private static readonly string[] formats = new string[]
        {
            DateUtils.YearLong,
            DateUtils.MonthShort,
            DateUtils.Day,
            DateUtils.Hour,
            DateUtils.Minute
        };

        public IPathBuilder PathBuilder { get; set; }

        public ReportOptions Report { get; set; }

        public ParallelOptions Parallel { get; set; }

        public RevitArguments Arguments { get; set; }

        public bool UseMetadata
        {
            get { return Arguments.RevitApp.UseMetadata; }
        }

        public TimeSpan Timeout
        {
            get { return Arguments.Timeout; }
        }

        public string RootDirectory { get; set; }

        public string JournalDirectory { get; set; }

        public DateTime JournalTimeDirectory { get; set; }

        public string ActionDirectory { get; set; }

        public bool DeleteRevitBackup { get; set; } = true;

        private PathCreator pathCreator;

        public bool CreateBackup
        {
            get { return pathCreator is object; }
        }

        public TaskOptions(IPathBuilder pathBuilder)
        {
            PathBuilder = pathBuilder ?? throw new ArgumentNullException(nameof(pathBuilder));
            Report = new ReportOptions();
            Parallel = new ParallelOptions();
            Arguments = new RevitArguments();
            RootDirectory = DirUtils.MyDocuments;
            JournalDirectory = DirUtils.MyDocuments;
            ActionDirectory = DirUtils.MyDocuments;
        }

        public RevitFamilyFile CreateBackupFile(RevitFamilyFile file)
        {
            return pathCreator.CreatePath(file);
        }

        public DirectoryNode GetJournalWorking()
        {
            var directory = PathBuilder.CreateRoot(JournalDirectory);
            var journalTime = JournalTimeDirectory;
            var timeFolderName = DateUtils.AsString(journalTime, Constant.Minus, formats);
            var timeFolder = PathBuilder.AddFolder(directory, timeFolderName);
            if (timeFolder.Exists() == false)
            {
                timeFolder.Create();
            }
            return timeFolder;
        }

        public PathCreator GetBackupSetting()
        {
            var creator = pathCreator;
            if (creator is null)
            {
                creator = new PathCreator(PathBuilder);
            }
            creator.SetRoot(RootDirectory);
            return creator;
        }

        public void SetBackupSetting(PathCreator creator)
        {
            pathCreator = creator ?? throw new ArgumentNullException(nameof(creator));
        }
    }
}
