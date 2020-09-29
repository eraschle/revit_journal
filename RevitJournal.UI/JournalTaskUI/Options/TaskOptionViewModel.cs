using DataSource;
using DataSource.Model.Product;
using RevitJournal.Revit;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.ObjectModel;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.Options
{
    public class TaskOptionViewModel : ANotifyPropertyChangedModel
    {
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
                NotifyPropertyChanged();
            }
        }

        public string TimeoutTitle
        {
            get { return CreateTimeoutTitle(); }
        }

        private string CreateTimeoutTitle()
        {
            return string.Concat(prefixTimeoutTitle, " [", DateUtils.AsString(Options.Timeout, format: DateUtils.Minute), " min]");
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
                NotifyPropertyChanged(nameof(TimeoutTitle));
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
                NotifyPropertyChanged();
            }
        }

        public string BackupSubFolder
        {
            get { return Options.Backup.BackupFolder; }
            set
            {
                if (StringUtils.Equals(Options.Backup.BackupFolder, value)) { return; }

                Options.Backup.BackupFolder = value;
                NotifyPropertyChanged();
            }
        }

        public string BackupSuffix
        {
            get { return Options.Backup.FileSuffix; }
            set
            {
                if (StringUtils.Equals(Options.Backup.FileSuffix, value)) { return; }

                Options.Backup.FileSuffix = value;
                NotifyPropertyChanged();
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
