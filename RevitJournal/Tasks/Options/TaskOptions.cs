using Autodesk.Revit.DB.Architecture;
using DataSource;
using DataSource.Models.FileSystem;
using DataSource.Models.Product;
using RevitJournal.Helper;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
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

        private static TaskOptions instance;

        public static TaskOptions Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new TaskOptions(PathFactory.Instance);
                }
                return instance;
            }
        }

        private readonly IPathBuilder pathBuilder;

        private readonly PathCreator pathCreator;

        public TaskOptionSelect<RevitApp> Applications { get; }

        public ITaskOption<bool> UseNewerApp { get; } = new TaskOptionBool(true);

        public ITaskOption<bool> LogResults { get; } = new TaskOptionBool(true);

        public ITaskOption<bool> LogSuccess { get; } = new TaskOptionBool(false);

        public ITaskOption<bool> LogError { get; } = new TaskOptionBool(true);

        public ITaskOptionRange Processes { get; } = new TaskOptionRange(Environment.ProcessorCount / 2, 1, Environment.ProcessorCount);

        public int ParallelProcesses
        {
            get { return (int)Processes.Value; }
        }

        public ITaskOptionRange ProcessTime { get; } = new TaskOptionRange(2, 1, 20);

        public ITaskOptionDirectory RootDirectory { get; }

        public ITaskOptionDirectory JournalDirectory { get; }

        public DateTime TimeDirectory { get; set; }

        public ITaskOptionDirectory ActionDirectory { get; }

        public TaskOptionBool DeleteRevitBackup { get; } = new TaskOptionBool(true);

        public ITaskOption<string> SymbolicPath { get; }

        public ITaskOptionDirectory NewRootPath { get; }

        public ITaskOption<string> FileSuffix { get; }

        public ITaskOption<string> BackupFolder { get; }

        public ITaskOption<bool> AddBackupAtEnd { get; }

        public ITaskOption<bool> CreateSourceBackup { get; } = new TaskOption<bool>(false);

        private TaskOptions(IPathBuilder builder)
        {
            pathBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
            pathCreator = new PathCreator(builder);
            RootDirectory = new TaskOptionPropertyDirectory(DirUtils.MyDocuments, pathCreator, nameof(pathCreator.RootPath), pathBuilder);
            ActionDirectory = new TaskOptionDirectory(DirUtils.MyDocuments, pathBuilder);
            JournalDirectory = new TaskOptionDirectory(DirUtils.MyDocuments, pathBuilder);
#if DEBUG
            RootDirectory = new TaskOptionPropertyDirectory(@"C:\develop\workspace\revit_journal_test_data\families", pathCreator, nameof(pathCreator.RootPath), pathBuilder);
            ActionDirectory = new TaskOptionDirectory(DirUtils.MyDocuments, pathBuilder);
            JournalDirectory = new TaskOptionDirectory(@"C:\develop\workspace\Content\journal", pathBuilder);
#endif
            Applications = new TaskOptionSelect<RevitApp>(ProductManager.UseMetadata, ProductManager.GetApplications(true));
            SymbolicPath = new TaskOptionProperty<string>(string.Empty, pathCreator, nameof(pathCreator.SymbolicPath));
            NewRootPath = new TaskOptionPropertyDirectory(string.Empty, pathCreator, nameof(pathCreator.NewRootPath), pathBuilder);
            BackupFolder = new TaskOptionProperty<string>(DateUtils.GetPathDate(), pathCreator, nameof(pathCreator.NewFolder));
            FileSuffix = new TaskOptionProperty<string>(DateUtils.GetPathDate(), pathCreator, nameof(pathCreator.FileSuffix));
            AddBackupAtEnd = new TaskOptionProperty<bool>(pathCreator.AddBackupAtEnd, pathCreator, nameof(pathCreator.AddBackupAtEnd));
        }

        public RevitFamilyFile CreateBackupFile(RevitFamilyFile file)
        {
            return pathCreator.CreatePath(file);
        }

        public DirectoryNode GetJournalWorking()
        {
            var directory = JournalDirectory.GetRootNode<TaskJournalFile>();
            var timeFolderName = DateUtils.GetPathDate(TimeDirectory, format: formats);
            var timeFolder = pathBuilder.AddFolder(directory, timeFolderName);
            if (timeFolder.Exists() == false)
            {
                timeFolder.Create();
            }
            return timeFolder;
        }

        public TimeSpan GetProcessTimeout()
        {
            return TimeSpan.FromMinutes(ProcessTime.Value);
        }


        public RevitApp GetApplication(RevitApp application)
        {
            if (application is null) { throw new ArgumentNullException(nameof(application)); }

            if (UseNewerApp.Value && Applications.Value.UseMetadata)
            {
                return ProductManager.GetVersionOrNewer(application.Version, onlyExist: true);
            }
            return application;
        }
    }
}
