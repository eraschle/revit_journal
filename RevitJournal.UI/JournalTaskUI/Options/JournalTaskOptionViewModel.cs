using DataSource;
using DataSource.Model.Product;
using RevitJournal.Journal;
using RevitJournal.Revit;
using RevitJournalUI.Helper;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RevitJournalUI.JournalTaskUI.Options
{
    public class JournalTaskOptionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string PrefixParallelProcess = "Parallel Processes";
        private const string PrefixTimeoutTitle = "Timeout";

        private readonly JournalManager TaskManager;

        public JournalTaskOptionViewModel(JournalManager manager)
        {
            TaskOption = new JournalOption();
            TaskManager = manager;
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

        public JournalOption TaskOption { get; private set; }

        public ObservableCollection<RevitApp> RevitApps { get; } = new ObservableCollection<RevitApp>();

        public RevitApp SelectedRevitApp
        {
            get { return TaskOption.RevitApp; }
            set
            {
                if (TaskOption.RevitApp != null && TaskOption.RevitApp.Equals(value)) { return; }

                TaskOption.RevitApp = value;
                OnPropertyChanged(nameof(SelectedRevitApp));
            }
        }

        private string _TimeoutTitle = CreateTimeoutTitle(JournalOption.DefaultTimeout);
        public string TimeoutTitle
        {
            get { return _TimeoutTitle; }
            set
            {
                if (_TimeoutTitle.Equals(value, StringComparison.CurrentCulture)) { return; }

                _TimeoutTitle = value;
                OnPropertyChanged(nameof(TimeoutTitle));
            }
        }

        private static string CreateTimeoutTitle(TimeSpan timeSpan)
        {
            return string.Concat(PrefixTimeoutTitle, " [", TimeSpanHelper.GetMinutes(timeSpan), " min]");
        }

        private string CreateTimeoutTitle()
        {
            return CreateTimeoutTitle(TaskOption.TaskTimeout);
        }

        public static int TimeoutMinimum
        {
            get { return (int)JournalOption.MinimumTimeout.TotalMinutes; }
        }

        public static int TimeoutMaximum
        {
            get { return (int)JournalOption.MaximumTimeout.TotalMinutes; }
        }

        public int Timeout
        {
            get { return (int)TaskOption.TaskTimeout.TotalMinutes; }
            set
            {
                var newValue = TimeSpan.FromMinutes(value);
                if (TaskOption.TaskTimeout == newValue) { return; }

                TaskOption.TaskTimeout = newValue;
                TimeoutTitle = CreateTimeoutTitle();
                OnPropertyChanged(nameof(Timeout));
            }
        }

        public bool LogResult
        {
            get { return TaskOption.LogResults; }
            set
            {
                if (TaskOption.LogResults == value) { return; }

                TaskOption.LogResults = value;
                OnPropertyChanged(nameof(LogResult));
                LogOptionsEnabled = value;
            }
        }

        public bool LogResultAll
        {
            get { return TaskOption.LogSuccess; }
            set
            {
                if (TaskOption.LogSuccess == value) { return; }

                TaskOption.LogSuccess = value;
                OnPropertyChanged(nameof(LogResultAll));
            }
        }

        public bool LogResultError
        {
            get { return TaskOption.LogError; }
            set
            {
                if (TaskOption.LogError == value) { return; }

                TaskOption.LogError = value;
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
            get { return JournalManager.MinParallelProcess; }
        }

        public int ParallelProcess
        {
            get { return TaskManager.ParallelProcess; }
            set
            {
                if (TaskManager.ParallelProcess == value) { return; }

                TaskManager.ParallelProcess = value;
                OnPropertyChanged(nameof(ParallelProcess));
                SetParallelProcessTitle();
            }
        }

        private string _ParallelProcessTitle = string.Empty;
        public string ParallelProcessTitle
        {
            get
            {
                if (string.IsNullOrEmpty(_ParallelProcessTitle))
                {
                    SetParallelProcessTitle();
                }
                return _ParallelProcessTitle;
            }
            set
            {
                if (_ParallelProcessTitle.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ParallelProcessTitle = value;
                OnPropertyChanged(nameof(ParallelProcessTitle));
            }
        }

        private bool _OptionsEnabled = true;
        public bool OptionsEnabled
        {
            get { return _OptionsEnabled; }
            set
            {
                if (_OptionsEnabled == value) { return; }

                _OptionsEnabled = value;
                OnPropertyChanged(nameof(OptionsEnabled));
                LogOptionsEnabled = value;
            }
        }

        private void SetParallelProcessTitle()
        {
            var title = PrefixParallelProcess + " [" + ParallelProcess + "]";
            ParallelProcessTitle = title;
        }

        public static int MaxParallelProcess
        {
            get { return JournalManager.MaxParallelProcess; }
        }

        public bool CreateBackup
        {
            get { return TaskOption.BackupRevitFile && OptionsEnabled; }
            set
            {
                if (TaskOption.BackupRevitFile == value) { return; }

                TaskOption.BackupRevitFile = value;
                OnPropertyChanged(nameof(CreateBackup));
            }
        }

        public string BackupSubFolder
        {
            get { return TaskOption.BackupSubFolder; }
            set
            {
                if (TaskOption.BackupSubFolder.Equals(value, StringComparison.CurrentCulture)) { return; }

                TaskOption.BackupSubFolder = value;
                OnPropertyChanged(nameof(BackupSubFolder));
            }
        }

        public string BackupSuffix
        {
            get { return TaskOption.BackupSuffix; }
            set
            {
                if (TaskOption.BackupSuffix.Equals(value, StringComparison.CurrentCulture)) { return; }

                TaskOption.BackupSuffix = value;
                OnPropertyChanged(nameof(BackupSuffix));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
