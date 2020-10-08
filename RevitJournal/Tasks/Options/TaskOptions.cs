using Autodesk.Revit.DB.Architecture;
using DataSource;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
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

        public TaskOptionSelect<RevitApp> Applications { get; }

        public TaskOptionBool LogResults { get; } = new TaskOptionBool(true);

        public TaskOptionBool LogSuccess { get; } = new TaskOptionBool(false);

        public TaskOptionBool LogError { get; } = new TaskOptionBool(true);

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

        public TaskOptionBool DeleteRevitBackup { get; } = new TaskOptionBool(true);

        public PathCreator PathCreator { get; }

        public TaskOptionProperty<string> SymbolicPath { get; }

        public TaskOptionProperty<string> NewRootPath { get; }

        public TaskOptionProperty<string> FileSuffix { get; }

        public TaskOptionProperty<string> BackupFolder { get; }

        public TaskOptionProperty<bool> AddBackupAtEnd { get; }

        public TaskOption<bool> CreateSourceBackup { get; } = new TaskOption<bool>(false);

        public TaskOptions(IPathBuilder builder)
        {
            pathBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
            PathCreator = new PathCreator(builder);
            Applications  = new TaskOptionSelect<RevitApp>(ProductManager.UseMetadata, ProductManager.GetApplications(true));
            Arguments = new RevitArguments();
            SymbolicPath = new TaskOptionProperty<string>(string.Empty, PathCreator, nameof(PathCreator.SymbolicPath));
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
