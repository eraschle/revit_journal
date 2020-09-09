using DataSource;
using DataSource.Model.Product;
using RevitJournal.Revit;
using RevitJournal.Tasks.Options;
using RevitJournalUI.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Utilities;

namespace RevitJournalUI.JournalTaskUI.Options
{
    public class TaskOptionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string prefixParallelProcess = "Parallel Processes";
        private const string prefixTimeoutTitle = "Timeout";

        public TaskOptions Options { get; set; }

        public TaskOptionViewModel()
        {
            Initialize();
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
                OnPropertyChanged(nameof(SelectedRevitApp));
            }
        }

        public string TimeoutTitle
        {
            get { return CreateTimeoutTitle(); }
        }

        private string CreateTimeoutTitle()
        {
            return string.Concat(prefixTimeoutTitle, " [", TimeSpanHelper.GetMinutes(Options.Timeout), " min]");
        }

        public static int TimeoutMinimum
        {
            get { return (int)RevitArguments.MinimumTimeout.TotalMinutes; }
        }

        public static int TimeoutMaximum
        {
            get { return (int)RevitArguments.MaximumTimeout.TotalMinutes; }
        }

        public int Timeout
        {
            get { return (int)Options.Timeout.TotalMinutes; }
            set
            {
                var newValue = TimeSpan.FromMinutes(value);
                if (Options.Timeout == newValue) { return; }

                Options.Arguments.Timeout = newValue;
                OnPropertyChanged(nameof(Timeout));
                OnPropertyChanged(nameof(TimeoutTitle));
            }
        }

        public bool LogResult
        {
            get { return Options.Report.LogResults; }
            set
            {
                if (Options.Report.LogResults == value) { return; }

                Options.Report.LogResults = value;
                OnPropertyChanged(nameof(LogResult));
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
                OnPropertyChanged(nameof(LogResultAll));
            }
        }

        public bool LogResultError
        {
            get { return Options.Report.LogError; }
            set
            {
                if (Options.Report.LogError == value) { return; }

                Options.Report.LogError = value;
                OnPropertyChanged(nameof(LogResultError));
            }
        }

        public bool LogOptionsEnabled
        {
            get { return LogResult && OptionsEnabled; }
            set
            {
                OnPropertyChanged(nameof(LogOptionsEnabled));
            }
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
                OnPropertyChanged(nameof(ParallelProcess));
                SetParallelProcessTitle();
            }
        }

        private string parallelProcessTitle = string.Empty;
        public string ParallelProcessTitle
        {
            get
            {
                if (string.IsNullOrEmpty(parallelProcessTitle))
                {
                    SetParallelProcessTitle();
                }
                return parallelProcessTitle;
            }
            set
            {
                if (StringUtils.Equals(parallelProcessTitle, value)) { return; }

                parallelProcessTitle = value;
                OnPropertyChanged(nameof(ParallelProcessTitle));
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
                OnPropertyChanged(nameof(OptionsEnabled));
                LogOptionsEnabled = value;
            }
        }

        private void SetParallelProcessTitle()
        {
            var title = prefixParallelProcess + " [" + ParallelProcess + "]";
            ParallelProcessTitle = title;
        }

        public static int MaxParallelProcess
        {
            get { return ParallelOptions.MaxProcesses; }
        }

        public bool CreateBackup
        {
            get { return Options.Backup.CreateBackup && OptionsEnabled; }
            set
            {
                if (Options.Backup.CreateBackup == value) { return; }

                Options.Backup.CreateBackup = value;
                OnPropertyChanged(nameof(CreateBackup));
            }
        }

        public string BackupSubFolder
        {
            get { return Options.Backup.BackupFolder; }
            set
            {
                if (StringUtils.Equals(Options.Backup.BackupFolder, value)) { return; }

                Options.Backup.BackupFolder = value;
                OnPropertyChanged(nameof(BackupSubFolder));
            }
        }

        public string BackupSuffix
        {
            get { return Options.Backup.FileSuffix; }
            set
            {
                if (StringUtils.Equals(Options.Backup.FileSuffix, value)) { return; }

                Options.Backup.FileSuffix = value;
                OnPropertyChanged(nameof(BackupSuffix));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
