using DataSource.Model.Product;
using DataSource;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.ObjectModel;
using Utilities.UI;
using DataSource.Model.FileSystem;

namespace RevitJournalUI.Pages.Settings
{
    public class SettingsPageModel : ANotifyPropertyChangedModel
    {
        public TaskOptions Options { get; set; } = new TaskOptions(PathFactory.Instance);

        public SettingsPageModel()
        {
            FamilyDirectory = new OptionDirectoryViewModel("Family Directory", Options.RootDirectory, true);
            JournalDirectory = new OptionDirectoryViewModel("Journal Directory", Options.JournalDirectory, true);
            ActionDirectory = new OptionDirectoryViewModel("Action Directory", Options.ActionDirectory, true);
            
            NewRootPath = new OptionDirectoryViewModel("New Root Directory", Options.NewRootPath, false, Options.RootDirectory);
            BackupFolder = new OptionStringViewModel("Backup Folder", Options.BackupFolder, false);
            AddBackupAtEnd = new OptionBoolViewModel("Add folder at the end", Options.AddBackupAtEnd, true);
            FileSuffix = new OptionStringViewModel("File Suffix", Options.FileSuffix, false);

            DeleteAppBackups = new OptionBoolViewModel("Delete Revit Backups", Options.DeleteRevitBackup, true);
            SourceBackup = new OptionBoolViewModel("Create source Backups", Options.CreateSourceBackup, true);

            LogResult = new OptionBoolViewModel ("Log Results",Options.LogResults, true);
            LogSuccess = new OptionBoolViewModel ("Log succes", Options.LogSuccess, true);
            LogError = new OptionBoolViewModel ("Log error", Options.LogError, true);

            ParallelProcess = new OptionSliderViewModel("Processes", Options.Processes, true);
            ProcessTimeout = new OptionSliderViewModel("Timeout", Options.ProcessTime, true, "min");
#if DEBUG
            FamilyDirectory.Value = @"C:\develop\workspace\revit_journal_test_data\families";
            JournalDirectory.Value = @"C:\develop\workspace\Content\journal";
#endif
            AddRevitApplications();
        }

        public OptionStringViewModel FamilyDirectory { get; }

        public OptionStringViewModel JournalDirectory { get; }

        public OptionStringViewModel ActionDirectory { get; }

        private void AddRevitApplications()
        {
            RevitApps.Clear();
            ProductManager.UpdateVersions();
            RevitApps.Add(ProductManager.UseMetadata);
            foreach (var revitApp in ProductManager.ExecutableRevitApps)
            {
                RevitApps.Add(revitApp);
            }
        }

        public ObservableCollection<RevitApp> RevitApps { get; } = new ObservableCollection<RevitApp>();

        public RevitApp SelectedRevitApp
        {
            get { return Options.Arguments.RevitApp; }
            set
            {
                if (Options.Arguments.RevitApp != null && Options.Arguments.RevitApp.Equals(value)) { return; }

                Options.Arguments.RevitApp = value;
                NotifyPropertyChanged();
            }
        }

        public OptionSliderViewModel ParallelProcess { get; }

        public OptionSliderViewModel ProcessTimeout { get; }

        public OptionBoolViewModel LogResult { get; }

        public OptionBoolViewModel LogSuccess { get; }

        public OptionBoolViewModel LogError { get; }

        public OptionBoolViewModel DeleteAppBackups { get; }

        #region Backup Source File

        public OptionBoolViewModel SourceBackup { get; }

        public OptionDirectoryViewModel NewRootPath { get; }

        public OptionBoolViewModel AddBackupAtEnd { get; }

        public OptionStringViewModel BackupFolder { get; }

        public OptionStringViewModel FileSuffix { get; }

        #endregion
    }
}
