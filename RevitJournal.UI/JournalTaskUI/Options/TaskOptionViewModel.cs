using DataSource;
using DataSource.Model.Product;
using RevitJournal.Revit;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.Options
{
    public class TaskOptionViewModel : ANotifyPropertyChangedModel
    {

        public TaskOptions Options { get; set; }

        public TaskOptionViewModel()
        {
            Initialize();
            CreateBackupCommand = new RelayCommand<object>(CreateBackupAction, CreateBackupPredicate);
        }

        private void Initialize()
        {
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

        public static TimeSpan TimeoutMinimum
        {
            get { return RevitArguments.MinimumTimeout; }
        }

        public static TimeSpan TimeoutMaximum
        {
            get { return RevitArguments.MaximumTimeout; }
        }

        public TimeSpan Timeout
        {
            get { return Options.Arguments.Timeout; }
            set
            {
                if (Options.Arguments.Timeout == value || value == TimeSpan.MinValue) { return; }

                Options.Arguments.Timeout = value;
                NotifyPropertyChanged();
            }
        }

        public bool LogResult
        {
            get { return Options.Report.LogResults; }
            set
            {
                if (Options.Report.LogResults == value) { return; }

                Options.Report.LogResults = value;
                NotifyPropertyChanged();
                LogOptionsEnabled = value;
            }
        }

        public bool LogResultAll
        {
            get { return Options.Report.LogSuccess; }
            set
            {
                if (Options.Report.LogSuccess == value) { return; }

                Options.Report.LogSuccess = value;
                NotifyPropertyChanged();
            }
        }

        public bool LogResultError
        {
            get { return Options.Report.LogError; }
            set
            {
                if (Options.Report.LogError == value) { return; }

                Options.Report.LogError = value;
                NotifyPropertyChanged();
            }
        }

        public bool LogOptionsEnabled
        {
            get { return LogResult && OptionsEnabled; }
            set { NotifyPropertyChanged(); }
        }

        public static int MinParallelProcess
        {
            get { return ParallelOptions.MinProcesses; }
        }

        public int ParallelProcess
        {
            get { return Options.Parallel.ParallelProcesses; }
            set
            {
                if (Options.Parallel.ParallelProcesses == value) { return; }

                Options.Parallel.ParallelProcesses = value;
                NotifyPropertyChanged();
            }
        }

        private bool optionsEnabled = true;
        public bool OptionsEnabled
        {
            get { return optionsEnabled; }
            set
            {
                if (optionsEnabled == value) { return; }

                optionsEnabled = value;
                NotifyPropertyChanged();
                LogOptionsEnabled = value;
            }
        }

        public static int MaxParallelProcess
        {
            get { return ParallelOptions.MaxProcesses; }
        }

        public ICommand CreateBackupCommand { get; }

        public bool CreateBackupPredicate(object parameter)
        {
            return OptionsEnabled && parameter is Window;
        }

        public void CreateBackupAction(object parameter)
        {
            var dialog = new BackupDialog
            {
                Owner = parameter as Window
            };
            var creator = Options.GetBackupSetting();
            dialog.Update(creator);
            if (dialog.ShowDialog() == true)
            {
                creator = dialog.GetPathCreator();
                Options.SetBackupSetting(creator);
            }
        }

        public bool DeleteAppBackups
        {
            get { return Options.DeleteRevitBackup; }
            set
            {
                if (Options.DeleteRevitBackup == value) { return; }

                Options.DeleteRevitBackup = value;
                NotifyPropertyChanged();
            }
        }
    }
}
