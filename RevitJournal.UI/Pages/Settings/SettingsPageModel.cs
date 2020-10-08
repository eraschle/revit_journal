using DataSource.Model.Product;
using DataSource;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.ObjectModel;
using Utilities.UI;
using DataSource.Model.FileSystem;
using System.Windows;
using System.Drawing;
using Utilities.System;
using System.ComponentModel;

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

            DeleteAppBackups = new OptionBoolViewModel("Delete App Backups", Options.DeleteRevitBackup, true);
            SourceBackup = new OptionBoolViewModel("Make Backups", Options.CreateSourceBackup, true);
            SourceBackup.PropertyChanged += SourceBackup_PropertyChanged;

            SymbolicPath = new OptionInformationViewModel("Backup path", Options.SymbolicPath, false);
            NewRootPath = new OptionDirectoryViewModel("New Root Directory", Options.NewRootPath, false, Options.RootDirectory);
            NewRootPath.PropertyChanged += PathOptions_PropertyChanged;
            BackupFolder = new OptionStringViewModel("Backup Folder", Options.BackupFolder, false);
            BackupFolder.PropertyChanged += PathOptions_PropertyChanged;
            BackupFolder.PropertyChanged += BackupFolder_PropertyChanged;
            AddBackupAtEnd = new OptionBoolViewModel("Add folder at the end", Options.AddBackupAtEnd, true);
            AddBackupAtEnd.PropertyChanged += PathOptions_PropertyChanged;
            FileSuffix = new OptionStringViewModel("File Suffix", Options.FileSuffix, false);
            FileSuffix.PropertyChanged += PathOptions_PropertyChanged;
            SetBackupOptionVisibility(SourceBackup.Value);
            SetAddFolderAtEndEnabled();

            RevitApps = new OptionSelectViewModel<RevitApp>("Application", Options.Applications, true);

            LogResult = new OptionBoolViewModel("Log Results", Options.LogResults, true);
            LogResult.PropertyChanged += LogResult_PropertyChanged;
            LogSuccess = new OptionBoolViewModel("Log succes", Options.LogSuccess, true);
            LogError = new OptionBoolViewModel("Log error", Options.LogError, true);
            SetOptionLogEnabled(LogResult.Value);

            ParallelProcess = new OptionSliderViewModel("Processes", Options.Processes, true, "CPU");
            ProcessTimeout = new OptionSliderViewModel("Timeout", Options.ProcessTime, true, "min");
#if DEBUG
            FamilyDirectory.Value = @"C:\develop\workspace\revit_journal_test_data\families";
            JournalDirectory.Value = @"C:\develop\workspace\Content\journal";
#endif
        }

        public OptionStringViewModel FamilyDirectory { get; }

        public OptionStringViewModel JournalDirectory { get; }

        public OptionStringViewModel ActionDirectory { get; }

        public OptionSelectViewModel<RevitApp> RevitApps { get; }

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

        #region Loggin results

        private void LogResult_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (StringUtils.Equals(args.PropertyName, nameof(LogResult.Value)) == false) { return; }

            SetOptionLogEnabled(LogResult.Value);
        }

        private void SetOptionLogEnabled(bool isChecked)
        {
            var visibility = GetVisibility(isChecked);
            LogError.OptionVisibility = visibility;
            LogSuccess.OptionVisibility = visibility;
        }

        public OptionBoolViewModel LogResult { get; }

        public OptionBoolViewModel LogSuccess { get; }

        public OptionBoolViewModel LogError { get; }

        #endregion


        private static Visibility GetVisibility(bool isChecked)
        {
            return isChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Backup File

        public OptionBoolViewModel DeleteAppBackups { get; }

        private void SourceBackup_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (StringUtils.Equals(args.PropertyName, nameof(SourceBackup.Value)) == false) { return; }

            SetBackupOptionVisibility(SourceBackup.Value);
        }

        private void PathOptions_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (StringUtils.Equals(args.PropertyName, nameof(SourceBackup.Value)) == false) { return; }

            SymbolicPath.InformationValue = string.Empty;
        }

        private void SetBackupOptionVisibility(bool isChecked)
        {
            var visibility = GetVisibility(isChecked);
            NewRootPath.OptionVisibility = visibility;
            AddBackupAtEnd.OptionVisibility = visibility;
            BackupFolder.OptionVisibility = visibility;
            FileSuffix.OptionVisibility = visibility;
            SymbolicPath.OptionVisibility = visibility;
        }

        public OptionInformationViewModel SymbolicPath { get; }

        public OptionBoolViewModel SourceBackup { get; }

        public OptionDirectoryViewModel NewRootPath { get; }

        public OptionStringViewModel BackupFolder { get; }

        private void BackupFolder_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (StringUtils.Equals(args.PropertyName, nameof(BackupFolder.Value)) == false) { return; }

            SetAddFolderAtEndEnabled();
        }

        private void SetAddFolderAtEndEnabled()
        {
            AddBackupAtEnd.IsEnabled = string.IsNullOrWhiteSpace(BackupFolder.Value) == false;
        }

        public OptionBoolViewModel AddBackupAtEnd { get; }

        public OptionStringViewModel FileSuffix { get; }

        #endregion
    }
}
