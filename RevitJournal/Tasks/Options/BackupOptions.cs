using DataSource.Model.FileSystem;
using RevitJournal.Helper;

namespace RevitJournal.Tasks.Options
{
    public class BackupOptions
    {
        private PathCreator PathCreator { get; } = new PathCreator(PathFactory.Instance);

        public bool CreateBackup { get; set; } = false;

        public RevitFamilyFile CreateBackupFile(RevitFamilyFile file)
        {
            return PathCreator.CreatePath(file);
        }

        public string RootPath
        {
            get { return PathCreator.RootPath; }
            set { PathCreator.SetRoot(value); }
        }

        public string FileSuffix
        {
            get { return PathCreator.FileSuffix; }
            set { PathCreator.FileSuffix = value; }
        }

        public string BackupFolder
        {
            get { return PathCreator.BackupFolder; }
            set { PathCreator.BackupFolder = value; }
        }

        public string NewRootPath
        {
            get { return PathCreator.NewRootPath; }
            set { PathCreator.SetNewRoot(value); }
        }

        public bool AddBackupAtEnd
        {
            get { return PathCreator.AddBackupAtEnd; }
            set { PathCreator.AddBackupAtEnd = value; }
        }
    }
}
