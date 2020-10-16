﻿using DataSource.Models.Product;
using RevitJournal.Tasks.Options;
using System.Windows;
using Utilities.System;
using System.ComponentModel;
using RevitJournalUI.Pages.Settings.Models;
using RevitJournalUI.Pages.Files.Worker;

namespace RevitJournalUI.Pages.Settings
{
    public class SettingsPageModel : APageModel
    {
        private readonly TaskOptions options = TaskOptions.Instance;

        public SettingsPageModel()
        {
            FamilyDirectory = new OptionDirectoryViewModel("Family Directory", options.RootDirectory, true, backgroundWorker: MetadataWorker.Create());
            JournalDirectory = new OptionDirectoryViewModel("Journal Directory", options.JournalDirectory, true);
            ActionDirectory = new OptionDirectoryViewModel("Action Directory", options.ActionDirectory, true);

            DeleteAppBackups = new OptionBoolViewModel("Delete App Backups", options.DeleteRevitBackup, true);
            SourceBackup = new OptionBoolViewModel("Make Backups", options.CreateSourceBackup, true);
            SourceBackup.PropertyChanged += SourceBackup_PropertyChanged;

            SymbolicPath = new OptionInformationViewModel("Backup path", options.SymbolicPath, false);
            NewRootPath = new OptionDirectoryViewModel("New Root Directory", options.NewRootPath, false, options.RootDirectory);
            NewRootPath.PropertyChanged += PathOptions_PropertyChanged;
            BackupFolder = new OptionStringViewModel("Backup Folder", options.BackupFolder, false);
            BackupFolder.PropertyChanged += PathOptions_PropertyChanged;
            BackupFolder.PropertyChanged += BackupFolder_PropertyChanged;
            AddBackupAtEnd = new OptionBoolViewModel("Add folder at the end", options.AddBackupAtEnd, true);
            AddBackupAtEnd.PropertyChanged += PathOptions_PropertyChanged;
            FileSuffix = new OptionStringViewModel("File Suffix", options.FileSuffix, false);
            FileSuffix.PropertyChanged += PathOptions_PropertyChanged;
            SetBackupOptionVisibility(SourceBackup.Value);
            SetAddFolderAtEndEnabled();

            Applications = new OptionSelectViewModel<RevitApp>("Application", options.Applications, true);
            UseNewerApplication = new OptionBoolViewModel("Use newer app", options.UseNewerApp, true);

            LogResult = new OptionBoolViewModel("Log Results", options.LogResults, true);
            LogResult.PropertyChanged += LogResult_PropertyChanged;
            LogSuccess = new OptionBoolViewModel("Log succes", options.LogSuccess, true);
            LogError = new OptionBoolViewModel("Log error", options.LogError, true);
            SetOptionLogEnabled(LogResult.Value);

            ParallelProcess = new OptionSliderViewModel("Processes", options.Processes, true, "CPU");
            ProcessTimeout = new OptionSliderViewModel("Timeout", options.ProcessTime, true, "min");
        }

        public override void SetModelData(object data) { }

        public OptionDirectoryViewModel FamilyDirectory { get; }

        public OptionDirectoryViewModel JournalDirectory { get; }

        public OptionDirectoryViewModel ActionDirectory { get; }

        public OptionSelectViewModel<RevitApp> Applications { get; }

        public OptionBoolViewModel UseNewerApplication { get; }

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
