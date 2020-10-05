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
            FamilyDirectory = new OptionDirectoryViewModel("Family Directory", Options.RootDirectory);
            JournalDirectory = new OptionDirectoryViewModel("Journal Directory", Options.JournalDirectory);
            ActionDirectory = new OptionDirectoryViewModel("Action Directory", Options.ActionDirectory);
            
            NewRootPath = new OptionDirectoryViewModel("New Root Directory", Options.NewRootPath, Options.RootDirectory);
            BackupFolder = new OptionStringViewModel("Backup Folder", Options.BackupFolder);
            AddBackupAtEnd = new OptionBoolViewModel("Add folder at the end", Options.AddBackupAtEnd);
            FileSuffix = new OptionStringViewModel("File Suffix", Options.FileSuffix);

            DeleteAppBackups = new OptionBoolViewModel("Delete Revit Backups", Options.DeleteRevitBackup);
            SourceBackup = new OptionBoolViewModel("Create source Backups", Options.CreateSourceBackup);

            LogResult = new OptionBoolViewModel ("Log Results",Options.LogResults);
            LogSuccess = new OptionBoolViewModel ("Log succes", Options.LogSuccess);
            LogError = new OptionBoolViewModel ("Log error", Options.LogError);

            ParallelProcess = new OptionSliderViewModel<int>("Processes", Options.ParallelProcesses);
            ProcessTimeout = new OptionSliderViewModel<TimeSpan>("Timeout", Options.ProcessTime);
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

        public OptionSliderViewModel<int> ParallelProcess { get; }

        public OptionSliderViewModel<TimeSpan> ProcessTimeout { get; }

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
