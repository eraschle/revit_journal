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

        private readonly PathCreator pathCreator;

        public TaskOptionSelect<RevitApp> Applications { get; }

        public TaskOptionBool UseNewerApp { get; } = new TaskOptionBool(true);

        public TaskOptionBool LogResults { get; } = new TaskOptionBool(true);

        public TaskOptionBool LogSuccess { get; } = new TaskOptionBool(false);

        public TaskOptionBool LogError { get; } = new TaskOptionBool(true);

        public TaskOptionRange Processes { get; } = new TaskOptionRange(Environment.ProcessorCount / 2, 1, Environment.ProcessorCount);

        public int ParallelProcesses
        {
            get { return (int)Processes.Value; }
        }

        public TaskOptionRange ProcessTime { get; } = new TaskOptionRange(2, 1, 20);

        public TaskOptionProperty<string> RootDirectory { get; }

        public TaskOption<string> JournalDirectory { get; } = new TaskOption<string>(DirUtils.MyDocuments);

        public DateTime TimeDirectory { get; set; }

        public TaskOption<string> ActionDirectory { get; } = new TaskOption<string>(DirUtils.MyDocuments);

        public TaskOptionBool DeleteRevitBackup { get; } = new TaskOptionBool(true);


        public TaskOptionProperty<string> SymbolicPath { get; }

        public TaskOptionProperty<string> NewRootPath { get; }

        public TaskOptionProperty<string> FileSuffix { get; }

        public TaskOptionProperty<string> BackupFolder { get; }

        public TaskOptionProperty<bool> AddBackupAtEnd { get; }

        public TaskOption<bool> CreateSourceBackup { get; } = new TaskOption<bool>(false);

        public TaskOptions(IPathBuilder builder)
        {
            pathBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
            pathCreator = new PathCreator(builder);
            Applications = new TaskOptionSelect<RevitApp>(ProductManager.UseMetadata, ProductManager.GetApplications(true));
            SymbolicPath = new TaskOptionProperty<string>(string.Empty, pathCreator, nameof(pathCreator.SymbolicPath));
            RootDirectory = new TaskOptionProperty<string>(DirUtils.MyDocuments, pathCreator, nameof(pathCreator.RootPath));
            NewRootPath = new TaskOptionProperty<string>(string.Empty, pathCreator, nameof(pathCreator.NewRootPath));
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
            var directory = pathBuilder.CreateRoot(JournalDirectory.Value);
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
            if(application is null) { throw new ArgumentNullException(nameof(application)); }

            if (UseNewerApp.Value && Applications.Value.UseMetadata)
            {
                return ProductManager.GetVersionOrNewer(application.Version, onlyExist: true);
            }
            return application;
        }
    }
}
