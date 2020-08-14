using DataSource;
using DataSource.Helper;
using RevitJournal.Journal;
using RevitJournal.Journal.Command;
using RevitJournalUI.Helper;
using RevitJournalUI.JournalTaskUI;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using RevitJournalUI.JournalTaskUI.JournalCommands;
using RevitJournalUI.JournalTaskUI.Models;
using RevitJournalUI.JournalTaskUI.Options;
using RevitJournalUI.MetadataUI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace RevitJournalUI.JournalManagerUI
{
    public class JournalManagerPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public const string PrefixCreateButton = "Create";
        public const string PrefixDuplicateButton = "Duplicate";
        public const string PrefixEditButton = "Edit";

        private readonly Progress<JournalResult> Progress;

        public TaskManager JournalManager { get; private set; }

        public JournalManagerPageModel()
        {
            JournalManager = new TaskManager();
            ProductManager.UpdateVersions();
            TaskOptionViewModel = new JournalTaskOptionViewModel(JournalManager);
            FamilyOverviewViewModel = new FamilyOverviewViewModel { FilterManager = FilterManager };
            PropertyChanged += new PropertyChangedEventHandler(FamilyOverviewViewModel.OnContentDirectoryChanged);
            CreateJournalCommand = new RelayCommand<FamilyOverviewViewModel>(CreateJournalCommandAction, CreateJournalCommandPredicate);
            BackJournalCommand = new RelayCommand<object>(BackJournalCommandAction);
            DuplicateCommand = new RelayCommand<FamilyOverviewViewModel>(DuplicateCommandAction, DuplicateCommandPredicate);
            EditCommand = new RelayCommand<FamilyOverviewViewModel>(EditCommandAction, EditCommandPredicate);
            ExecuteJournalCommand = new RelayCommand<object>(ExecuteJournalCommandAction, ExecuteJournalCommandPredicate);
            CancelJournalCommand = new RelayCommand<object>(CancelJournalCommandAction, CancelJournalCommandPredicate);

            Progress = new Progress<JournalResult>();
            FamilyOverviewViewModel.PropertyChanged += new PropertyChangedEventHandler(OnCheckedChanged);

            SetupFilterCommand = new RelayCommand<ObservableCollection<DirectoryViewModel>>(SetupFilterCommandAction);
            SetupJournalCommand = new RelayCommand<TaskManager>(SetupJournalCommandAction);

#if DEBUG
            FamilyDirectory = @"C:\develop\workspace\_my-work\RevitJournal\data\test files";
            JournalDirectory = @"C:\develop\workspace\_my-work\RevitJournal\data\meta_data";
#else
            var myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FamilyDirectory = myDocument;
            JournalDirectory = myDocument;
#endif
            TaskOptionViewModel.SelectedRevitApp = ProductManager.UseMetadata;
        }


        #region Common Settings

        public JournalTaskOptionViewModel TaskOptionViewModel { get; }

        private string _FamilyDirectory = string.Empty;
        public string FamilyDirectory
        {
            get { return _FamilyDirectory; }
            set
            {
                if (_FamilyDirectory.Equals(value, StringComparison.CurrentCulture)) { return; }

                _FamilyDirectory = value;
                OnPropertyChanged(nameof(FamilyDirectory));
            }
        }

        public ICommand ChooseFamilyDirectoryCommand
        {
            get { return new RelayCommand<string>(ChooseFamilyDirectoryAction); }
        }

        private void ChooseFamilyDirectoryAction(string selectedPath)
        {
            FamilyDirectory = ChooseDirectory(selectedPath);
        }

        public string JournalDirectory
        {
            get { return JournalManager.JournalDirectory; }
            set
            {
                if (JournalManager.JournalDirectory.Equals(value, StringComparison.CurrentCulture)) { return; }

                JournalManager.JournalDirectory = value;
                OnPropertyChanged(nameof(JournalDirectory));
            }
        }

        public ICommand ChooseJournalDirectoryCommand
        {
            get { return new RelayCommand<string>(ChooseJournalDirectoryAction); }
        }

        private void ChooseJournalDirectoryAction(string selectedPath)
        {
            JournalDirectory = ChooseDirectory(selectedPath);
        }

        private static string ChooseDirectory(string selectedPath)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = selectedPath;
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return selectedPath;
        }

        #endregion


        public void SetProgress(int percent)
        {
            LoadingProcessPercent = percent;
            LoadingProcessTitel = $"Loading [{percent}%]";
        }

        private string _LoadingProcessTitel;
        public string LoadingProcessTitel
        {
            get { return _LoadingProcessTitel; }
            set
            {
                if (_LoadingProcessTitel == value) { return; }

                _LoadingProcessTitel = value;
                OnPropertyChanged(nameof(LoadingProcessTitel));
            }
        }

        private int _LoadingProcessPercent;
        public int LoadingProcessPercent
        {
            get { return _LoadingProcessPercent; }
            set
            {
                if (_LoadingProcessPercent == value) { return; }

                _LoadingProcessPercent = value;
                OnPropertyChanged(nameof(LoadingProcessPercent));
            }
        }

        #region Filtering


        private Visibility _SetupFilterVisibility = Visibility.Collapsed;
        public Visibility SetupFilterVisibility
        {
            get { return _SetupFilterVisibility; }
            set
            {
                if (_SetupFilterVisibility == value) { return; }

                _SetupFilterVisibility = value;
                OnPropertyChanged(nameof(SetupFilterVisibility));
            }
        }

        public ICommand SetupFilterCommand { get; }

        private FilterManager FilterManager { get; } = new FilterManager();

        private void SetupFilterCommandAction(ObservableCollection<DirectoryViewModel> viewModels)
        {
            FilterManager.ClearFilter();
            var dialog = new RevitFamilyFilterView(viewModels, FilterManager);
            if (dialog.ShowDialog() == true)
            {
                FilterManager.UpdateFilter(dialog.ViewModel);
                UpdateCheckedFilters();
                FamilyOverviewViewModel.FilterUpdated(FilterManager);
            }
        }

        public ObservableCollection<FilterViewModel> FilterViewModels { get; }
            = new ObservableCollection<FilterViewModel>();

        public void UpdateCheckedFilters()
        {
            FilterViewModels.Clear();
            foreach (var filter in FilterManager.CheckedFilterViewModels())
            {
                FilterViewModels.Add(filter);
            }
        }

        #endregion

        private Visibility _SetupJournalVisibility = Visibility.Visible;
        public Visibility SetupJournalVisibility
        {
            get { return _SetupJournalVisibility; }
            set
            {
                if (_SetupJournalVisibility == value) { return; }

                _SetupJournalVisibility = value;
                OnPropertyChanged(nameof(SetupJournalVisibility));
            }
        }

        public ICommand SetupJournalCommand { get; }

        private JournalManagerViewModel JournalCommandManagerModel;

        private void SetupJournalCommandAction(TaskManager manager)
        {
            var dialog = new JournalManagerView(manager);
            JournalCommandManagerModel = dialog.ViewModel;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                UpdateJournalCommands();
            }
        }
        public ObservableCollection<IJournalCommand> JournalCommands { get; }
            = new ObservableCollection<IJournalCommand>();

        private void UpdateJournalCommands()
        {
            JournalCommands.Clear();
            foreach (var commands in JournalCommandManagerModel.CheckedCommands)
            {
                JournalCommands.Add(commands);
            }
        }

        #region Revit Content Overview

        public FamilyOverviewViewModel FamilyOverviewViewModel { get; }

        private Visibility _RevitFilesVisibility = Visibility.Visible;
        public Visibility RevitFilesVisibility
        {
            get { return _RevitFilesVisibility; }
            set
            {
                if (_RevitFilesVisibility == value) { return; }

                _RevitFilesVisibility = value;
                OnPropertyChanged(nameof(RevitFilesVisibility));
            }
        }

        private string _CreateButtonName = PrefixCreateButton;
        public string CreateButtonName
        {
            get { return _CreateButtonName; }
            set
            {
                if (_CreateButtonName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _CreateButtonName = value;
                OnPropertyChanged(nameof(CreateButtonName));
            }
        }

        private void OnCheckedChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args is null) { return; }
            if (args.PropertyName.Equals(nameof(FamilyOverviewViewModel.CheckedRevitFilesCount), StringComparison.CurrentCulture) == false
                && args.PropertyName.Equals(nameof(FamilyOverviewViewModel.ValidCheckedRevitFilesCount), StringComparison.CurrentCulture) == false) { return; }

            CreateButtonName = $"{PrefixCreateButton} [{FamilyOverviewViewModel.CheckedRevitFilesCount}]";
            UpdateDuplicateButtonName();
            UpdateEditButtonName();
        }

        public void UpdateDuplicateButtonName()
        {
            DuplicateButtonName = $"{PrefixDuplicateButton} [{FamilyOverviewViewModel.ValidCheckedRevitFilesCount}]";
        }

        public void UpdateEditButtonName()
        {
            EditButtonName = $"{PrefixEditButton} [{FamilyOverviewViewModel.EditableRevitFilesCount}]";
        }

        public ICommand CreateJournalCommand { get; private set; }

        private bool CreateJournalCommandPredicate(FamilyOverviewViewModel model)
        {
            return model != null
                && model.HasCheckedRevitFiles
                && JournalCommandManagerModel != null
                && JournalCommandManagerModel.HasCheckedCommands;
        }

        private void CreateJournalCommandAction(FamilyOverviewViewModel model)
        {
            JournalManager.ClearTasks();
            var taskOptions = TaskOptionViewModel.TaskOption;
            var useMetadataRevitVersion = taskOptions.RevitApp.UseMetadata;
            var journalCommands = JournalCommandManagerModel.CheckedCommands;

            const bool runnungApps = true;
            foreach (var family in model.RecursiveRevitFamilies)
            {
                if (family.HasRepairableMetadata == false
                    || (useMetadataRevitVersion && family.HasValidMetadata == false)) { continue; }

                if (useMetadataRevitVersion)
                {
                    var metadata = family.Metadata;
                    if (metadata.HasProduct(out var product) == false
                        || ProductManager.HasVersionOrNewer(product.Version, runnungApps) == false)
                    {
                        Debug.WriteLine("No vaid Product Version installed.");
                        continue;
                    }

                    var revitApp = ProductManager.GetVersionOrNewer(product.Version, runnungApps);
                    taskOptions.RevitApp = revitApp;
                }
                var journalTask = new RevitTask(family, taskOptions);
                foreach (var command in journalCommands)
                {
                    journalTask.AddCommand(command);
                }

                JournalManager.AddTask(journalTask);
            }

            JournalManager.CreateJournalTaskRunner(Progress);
            JournalTaskOverviewViewModel.Update(JournalManager);

            RevitFilesVisibility = Visibility.Collapsed;
            BackButtonVisibility = Visibility.Visible;
            JournalCommandVisibility = Visibility.Visible;
            SetupJournalVisibility = Visibility.Collapsed;
            SetupFilterVisibility = Visibility.Collapsed;
            TaskOptionViewModel.OptionsEnabled = false;
        }

        public ICommand BackJournalCommand { get; private set; }

        private void BackJournalCommandAction(object parameter)
        {
            JournalManager.CleanJournalTaskRunner();
            RevitFilesVisibility = Visibility.Visible;
            JournalCommandVisibility = Visibility.Collapsed;
            BackButtonVisibility = Visibility.Collapsed;

            SetupJournalVisibility = Visibility.Visible;
            SetupFilterVisibility = Visibility.Visible;
            TaskOptionViewModel.OptionsEnabled = true;
        }

        private Visibility _BackButtonVisibility = Visibility.Collapsed;
        public Visibility BackButtonVisibility
        {
            get { return _BackButtonVisibility; }
            set
            {
                if (_BackButtonVisibility == value) { return; }

                _BackButtonVisibility = value;
                OnPropertyChanged(nameof(BackButtonVisibility));
            }
        }

        private string _DuplicateButtonName = PrefixDuplicateButton;
        public string DuplicateButtonName
        {
            get { return _DuplicateButtonName; }
            set
            {
                if (_DuplicateButtonName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _DuplicateButtonName = value;
                OnPropertyChanged(nameof(DuplicateButtonName));
            }
        }

        public ICommand DuplicateCommand { get; private set; }

        private bool DuplicateCommandPredicate(FamilyOverviewViewModel model)
        {
            return model != null && model.HasCheckedAndValidRevitFiles;
        }

        private void DuplicateCommandAction(FamilyOverviewViewModel model)
        {
            var validModels = model.ValidRecursiveRevitFamilies;
            var dialog = new MetadataDuplicateDialog(validModels)
            {
                SizeToContent = SizeToContent.WidthAndHeight
            };
            dialog.ShowDialog();
        }

        private string _EditButtonName = PrefixEditButton;
        public string EditButtonName
        {
            get { return _EditButtonName; }
            set
            {
                if (_EditButtonName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _EditButtonName = value;
                OnPropertyChanged(nameof(EditButtonName));
            }
        }

        public ICommand EditCommand { get; private set; }

        private bool EditCommandPredicate(FamilyOverviewViewModel model)
        {
            return model != null && model.HasEditableRevitFiles;
        }

        private void EditCommandAction(FamilyOverviewViewModel model)
        {
            var validModels = model.EditableRecursiveRevitFamilies;
            var dialog = new MetadataEditDialogView(validModels)
            {
                SizeToContent = SizeToContent.WidthAndHeight
            };
            dialog.ShowDialog();
        }

        #endregion

        #region Journal Tasks Overview

        public JournalTaskOverviewViewModel JournalTaskOverviewViewModel { get; } = new JournalTaskOverviewViewModel();

        private Visibility _JournalTaskVisibility = Visibility.Collapsed;
        public Visibility JournalCommandVisibility
        {
            get { return _JournalTaskVisibility; }
            set
            {
                if (_JournalTaskVisibility == value) { return; }

                _JournalTaskVisibility = value;
                OnPropertyChanged(nameof(JournalCommandVisibility));
            }
        }

        #endregion

        #region Execute Tasks

        public ICommand ExecuteJournalCommand { get; private set; }

        private async void ExecuteJournalCommandAction(object parameter)
        {
            RunButtonEnable = false;
            CancelButtonVisibility = Visibility.Visible;
            BackButtonVisibility = Visibility.Collapsed;

            Progress.ProgressChanged += new EventHandler<JournalResult>(OnReport);
            using (var cancel = new CancellationTokenSource())
            {
                Cancellation = cancel;
                await JournalManager.ExecuteAllTaskAsync(Cancellation.Token)
                                    .ConfigureAwait(false);
                Cancellation = null;
            }
            Progress.ProgressChanged -= OnReport;
            RunButtonEnable = true;
        }

        private bool ExecuteJournalCommandPredicate(object parameter)
        {
            return JournalManager.HasRevitTasks;
        }

        private void OnReport(object sender, JournalResult result)
        {
            if (result is null) { return; }

            JournalTaskOverviewViewModel.SetResult(JournalManager, result);
        }

        private bool _RunButtonEnable = true;
        public bool RunButtonEnable
        {
            get { return _RunButtonEnable; }
            set
            {
                if (_RunButtonEnable == value) { return; }

                _RunButtonEnable = value;
                OnPropertyChanged(nameof(RunButtonEnable));
            }
        }

        #endregion

        #region Cancel Task Execution

        private CancellationTokenSource _Cancellation;
        public CancellationTokenSource Cancellation
        {
            get { return _Cancellation; }
            set
            {
                _Cancellation = value;
                OnPropertyChanged(nameof(Cancellation));
                CancelButtonEnable = _Cancellation != null;
            }
        }

        private bool _CancelButtonEnable = false;
        public bool CancelButtonEnable
        {
            get { return _CancelButtonEnable; }
            set
            {
                if (_CancelButtonEnable == value) { return; }

                _CancelButtonEnable = value;
                OnPropertyChanged(nameof(CancelButtonEnable));
            }
        }

        public ICommand CancelJournalCommand { get; private set; }

        private void CancelJournalCommandAction(object parameter)
        {
            try
            {
                if (Cancellation is null) { return; }

                Cancellation.Cancel(false);
            }
            finally
            {
                Cancellation = null;
            }
        }

        private bool CancelJournalCommandPredicate(object parameter)
        {
            return Cancellation != null;
        }


        private Visibility _CancelButtonVisibility = Visibility.Collapsed;
        public Visibility CancelButtonVisibility
        {
            get { return _CancelButtonVisibility; }
            set
            {
                if (_CancelButtonVisibility == value) { return; }

                _CancelButtonVisibility = value;
                OnPropertyChanged(nameof(CancelButtonVisibility));
            }
        }

        #endregion


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
