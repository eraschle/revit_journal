using Autodesk.Revit.DB.Architecture;
using DataSource.Model.FileSystem;
using RevitJournal.Helper;
using RevitJournal.Revit;
using RevitJournal.Tasks.Options.Parameter;
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

        private readonly IPathBuilder pathBuilder;

        public BoolOption LogResults { get; } = new BoolOption(true);

        public BoolOption LogSuccess { get; } = new BoolOption(false);

        public BoolOption LogError { get; } = new BoolOption(true);

        public TaskOptionRange Processes { get; } = new TaskOptionRange(Environment.ProcessorCount / 2, 1, Environment.ProcessorCount);

        public int ParallelProcesses
        {
            get { return (int)Processes.Value; }
        }

        public TaskOptionRange ProcessTime { get; } = new TaskOptionRange(2, 1, 20);

        public TimeSpan ProcessTimeout
        {
            get { return TimeSpan.FromMinutes(ProcessTime.Value); }
        }

        public RevitArguments Arguments { get; set; }

        public bool UseMetadata
        {
            get { return Arguments.RevitApp.UseMetadata; }
        }

        public TaskOptionProperty<string> RootDirectory { get; }

        public TaskOption<string> JournalDirectory { get; } = new TaskOption<string>(DirUtils.MyDocuments);

        public DateTime TimeDirectory { get; set; }

        public TaskOption<string> ActionDirectory { get; } = new TaskOption<string>(DirUtils.MyDocuments);

        public BoolOption DeleteRevitBackup { get; } = new BoolOption(true);

        public PathCreator PathCreator { get; }

        public TaskOptionProperty<string> NewRootPath { get; }

        public TaskOptionProperty<string> FileSuffix { get; }

        public TaskOptionProperty<string> BackupFolder { get; }

        public TaskOptionProperty<bool> AddBackupAtEnd { get; }

        public TaskOption<bool> CreateSourceBackup { get; } = new TaskOption<bool>(false);

        public TaskOptions(IPathBuilder builder)
        {
            pathBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
            PathCreator = new PathCreator(builder);
            Arguments = new RevitArguments();
            RootDirectory = new TaskOptionProperty<string>(DirUtils.MyDocuments, PathCreator, nameof(PathCreator.RootPath));
            NewRootPath = new TaskOptionProperty<string>(string.Empty, PathCreator, nameof(PathCreator.NewRootPath));
            BackupFolder = new TaskOptionProperty<string>(DateUtils.GetPathDate(), PathCreator, nameof(PathCreator.NewFolder));
            FileSuffix = new TaskOptionProperty<string>(DateUtils.GetPathDate(), PathCreator, nameof(PathCreator.FileSuffix));
            AddBackupAtEnd = new TaskOptionProperty<bool>(PathCreator.AddBackupAtEnd, PathCreator, nameof(PathCreator.AddBackupAtEnd));
        }

        public RevitFamilyFile CreateBackupFile(RevitFamilyFile file)
        {
            return PathCreator.CreatePath(file);
        }

        public DirectoryNode GetJournalWorking()
        {
            var directory = pathBuilder.CreateRoot(JournalDirectory.Value);
            var timeFolderName = DateUtils.GetPathDate(TimeDirectory, format: formats);
            var timeFolder = pathBuilder.AddFolder(directory, timeFolderName);
            if (timeFolder.Exists() == false)
            {
                timeFolder.Create();
            }
            return timeFolder;
        }
    }
}
