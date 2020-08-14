using DataSource;
using RevitAction.Action;
using RevitJournal.Journal;
using RevitJournal.Tasks;
using RevitJournalUI.JournalTaskUI;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using RevitJournalUI.JournalTaskUI.Models;
using RevitJournalUI.JournalTaskUI.Options;
using RevitJournalUI.MetadataUI;
using RevitJournalUI.Tasks.Actions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Utilities;
using Utilities.UI.Helper;

namespace RevitJournalUI.JournalManagerUI
{
    public class JournalManagerPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public const string PrefixCreateButton = "Create";
        public const string PrefixDuplicateButton = "Duplicate";
        public const string PrefixEditButton = "Edit";

        private readonly Progress<JournalResult> Progress;

        public TaskManager TaskManager { get; private set; }
        private readonly TaskOptions taskOptions;

        public JournalManagerPageModel()
        {
            TaskManager = new TaskManager();
            taskOptions = new TaskOptions();
            ProductManager.UpdateVersions();
            TaskOptionViewModel = new TaskOptionViewModel { Options = taskOptions };
            FamiliesViewModel = new FamilyOverviewViewModel { FilterManager = FilterManager };
            PropertyChanged += new PropertyChangedEventHandler(FamiliesViewModel.OnContentDirectoryChanged);
            CreateCommand = new RelayCommand<FamilyOverviewViewModel>(CreateCommandAction, CreateCommandPredicate);
            BackCommand = new RelayCommand<object>(BackCommandAction);
            DuplicateCommand = new RelayCommand<FamilyOverviewViewModel>(DuplicateCommandAction, DuplicateCommandPredicate);
            EditCommand = new RelayCommand<FamilyOverviewViewModel>(EditCommandAction, EditCommandPredicate);
            ExecuteCommand = new RelayCommand<object>(ExecuteCommandAction, ExecuteCommandPredicate);
            CancelCommand = new RelayCommand<object>(CancelCommandAction, CancelCommandPredicate);

            Progress = new Progress<JournalResult>();
            FamiliesViewModel.PropertyChanged += new PropertyChangedEventHandler(OnCheckedChanged);

            SetupFilterCommand = new RelayCommand<ObservableCollection<DirectoryViewModel>>(SetupFilterCommandAction);
            SetupJournalCommand = new RelayCommand<TaskManager>(SetupJournalCommandAction);

#if DEBUG
            FamilyDirectory = @"C:\develop\workspace\test_data\families";
            JournalDirectory = @"C:\develop\workspace\test_data\journals";
#else
            var myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FamilyDirectory = myDocument;
            JournalDirectory = myDocument;
#endif
            TaskOptionViewModel.SelectedRevitApp = ProductManager.UseMetadata;
        }


        #region Common Settings

        public TaskOptionViewModel TaskOptionViewModel { get; }

        private string familyDirectory = string.Empty;
        public string FamilyDirectory
        {
            get { return familyDirectory; }
            set
            {
                if (StringUtils.Equals(familyDirectory, value)) { return; }

                familyDirectory = value;
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
            get { return taskOptions.Common.JournalDirectory; }
            set
            {
                if (StringUtils.Equals(taskOptions.Common.JournalDirectory, value)) { return; }

                taskOptions.Common.JournalDirectory = value;
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

        private string loadingProcessTitel;
        public string LoadingProcessTitel
        {
            get { return loadingProcessTitel; }
            set
            {
                if (loadingProcessTitel == value) { return; }

                loadingProcessTitel = value;
                OnPropertyChanged(nameof(LoadingProcessTitel));
            }
        }

        private int loadingProcessPercent;
        public int LoadingProcessPercent
        {
            get { return loadingProcessPercent; }
            set
            {
                if (loadingProcessPercent == value) { return; }

                loadingProcessPercent = value;
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
                FamiliesViewModel.FilterUpdated(FilterManager);
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

        private Visibility setupJournalVisibility = Visibility.Visible;
        public Visibility SetupJournalVisibility
        {
            get { return setupJournalVisibility; }
            set
            {
                if (setupJournalVisibility == value) { return; }

                setupJournalVisibility = value;
                OnPropertyChanged(nameof(SetupJournalVisibility));
            }
        }

        public ICommand SetupJournalCommand { get; }

        private TaskActionsViewModel actionManagerModel;

        private void SetupJournalCommandAction(TaskManager manager)
        {
            var dialog = new TaskActionsView(manager);
            actionManagerModel = dialog.ViewModel;
            var result = dialog.ShowDialog();
            if (result == true)
            {
                UpdateJournalCommands();
            }
        }
        public ObservableCollection<ITaskAction> Actions { get; }
            = new ObservableCollection<ITaskAction>();

        private void UpdateJournalCommands()
        {
            Actions.Clear();
            foreach (var commands in actionManagerModel.CheckedActions)
            {
                Actions.Add(commands);
            }
        }

        #region Revit Content Overview

        public FamilyOverviewViewModel FamiliesViewModel { get; }

        private Visibility familiesVisibility = Visibility.Visible;
        public Visibility FamiliesVisibility
        {
            get { return familiesVisibility; }
            set
            {
                if (familiesVisibility == value) { return; }

                familiesVisibility = value;
                OnPropertyChanged(nameof(FamiliesVisibility));
            }
        }

        private string createName = PrefixCreateButton;
        public string CreateName
        {
            get { return createName; }
            set
            {
                if (createName.Equals(value, StringComparison.CurrentCulture)) { return; }

                createName = value;
                OnPropertyChanged(nameof(CreateName));
            }
        }

        private void OnCheckedChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args is null) { return; }
            if (StringUtils.Equals(args.PropertyName, nameof(FamiliesViewModel.CheckedRevitFilesCount)) == false
                && StringUtils.Equals(args.PropertyName, nameof(FamiliesViewModel.ValidCheckedRevitFilesCount)) == false) { return; }

            CreateName = $"{PrefixCreateButton} [{FamiliesViewModel.CheckedRevitFilesCount}]";
            UpdateDuplicateButtonName();
            UpdateEditButtonName();
        }

        public void UpdateDuplicateButtonName()
        {
            DuplicateName = $"{PrefixDuplicateButton} [{FamiliesViewModel.ValidCheckedRevitFilesCount}]";
        }

        public void UpdateEditButtonName()
        {
            EditName = $"{PrefixEditButton} [{FamiliesViewModel.EditableRevitFilesCount}]";
        }

        public ICommand CreateCommand { get; private set; }

        private bool CreateCommandPredicate(FamilyOverviewViewModel model)
        {
            return model != null
                && model.HasCheckedRevitFiles
                && actionManagerModel != null
                && actionManagerModel.HasCheckedCommands;
        }

        private void CreateCommandAction(FamilyOverviewViewModel model)
        {
            TaskManager.ClearTasks();
            var arguments = taskOptions.Arguments;
            var useMetadataRevitVersion = arguments.RevitApp.UseMetadata;

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
                    arguments.RevitApp = revitApp;
                }
                var task = new RevitTask(family);
                foreach (var action in actionManagerModel.CheckedActions)
                {
                    task.AddAction(action);
                }

                TaskManager.AddTask(task);
            }

            TaskManager.CreateTaskRunner(Progress);
            TasksViewModel.Update(TaskManager);

            FamiliesVisibility = Visibility.Collapsed;
            BackVisibility = Visibility.Visible;
            TaskVisibility = Visibility.Visible;
            SetupJournalVisibility = Visibility.Collapsed;
            SetupFilterVisibility = Visibility.Collapsed;
            TaskOptionViewModel.OptionsEnabled = false;
        }

        public ICommand BackCommand { get; private set; }

        private void BackCommandAction(object parameter)
        {
            TaskManager.CleanTaskRunner();
            FamiliesVisibility = Visibility.Visible;
            TaskVisibility = Visibility.Collapsed;
            BackVisibility = Visibility.Collapsed;

            SetupJournalVisibility = Visibility.Visible;
            SetupFilterVisibility = Visibility.Visible;
            TaskOptionViewModel.OptionsEnabled = true;
        }

        private Visibility backVisibility = Visibility.Collapsed;
        public Visibility BackVisibility
        {
            get { return backVisibility; }
            set
            {
                if (backVisibility == value) { return; }

                backVisibility = value;
                OnPropertyChanged(nameof(BackVisibility));
            }
        }

        private string duplicateName = PrefixDuplicateButton;
        public string DuplicateName
        {
            get { return duplicateName; }
            set
            {
                if (duplicateName.Equals(value, StringComparison.CurrentCulture)) { return; }

                duplicateName = value;
                OnPropertyChanged(nameof(DuplicateName));
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

        private string editName = PrefixEditButton;
        public string EditName
        {
            get { return editName; }
            set
            {
                if (editName.Equals(value, StringComparison.CurrentCulture)) { return; }

                editName = value;
                OnPropertyChanged(nameof(EditName));
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

        public JournalTaskOverviewViewModel TasksViewModel { get; } = new JournalTaskOverviewViewModel();

        private Visibility taskVisibility = Visibility.Collapsed;
        public Visibility TaskVisibility
        {
            get { return taskVisibility; }
            set
            {
                if (taskVisibility == value) { return; }

                taskVisibility = value;
                OnPropertyChanged(nameof(TaskVisibility));
            }
        }

        #endregion

        #region Execute Tasks

        public ICommand ExecuteCommand { get; private set; }

        private async void ExecuteCommandAction(object parameter)
        {
            ExecuteEnable = false;
            CancelVisibility = Visibility.Visible;
            BackVisibility = Visibility.Collapsed;

            Progress.ProgressChanged += new EventHandler<JournalResult>(OnReport);
            using (var cancel = new CancellationTokenSource())
            {
                Cancellation = cancel;
                await TaskManager.ExecuteTasks(taskOptions, Cancellation.Token)
                                 .ConfigureAwait(false);
                Cancellation = null;
            }
            Progress.ProgressChanged -= OnReport;
            ExecuteEnable = true;
        }

        private bool ExecuteCommandPredicate(object parameter)
        {
            return TaskManager.HasRevitTasks;
        }

        private void OnReport(object sender, JournalResult result)
        {
            if (result is null) { return; }

            TasksViewModel.SetResult(TaskManager, result);
        }

        private bool executeEnable = true;
        public bool ExecuteEnable
        {
            get { return executeEnable; }
            set
            {
                if (executeEnable == value) { return; }

                executeEnable = value;
                OnPropertyChanged(nameof(ExecuteEnable));
            }
        }

        #endregion

        #region Cancel Task Execution

        private CancellationTokenSource cancellation;
        public CancellationTokenSource Cancellation
        {
            get { return cancellation; }
            set
            {
                cancellation = value;
                OnPropertyChanged(nameof(Cancellation));
                CancelEnable = cancellation != null;
            }
        }

        private bool cancelEnable = false;
        public bool CancelEnable
        {
            get { return cancelEnable; }
            set
            {
                if (cancelEnable == value) { return; }

                cancelEnable = value;
                OnPropertyChanged(nameof(CancelEnable));
            }
        }

        public ICommand CancelCommand { get; private set; }

        private void CancelCommandAction(object parameter)
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

        private bool CancelCommandPredicate(object parameter)
        {
            return Cancellation != null;
        }


        private Visibility cancelVisibility = Visibility.Collapsed;
        public Visibility CancelVisibility
        {
            get { return cancelVisibility; }
            set
            {
                if (cancelVisibility == value) { return; }

                cancelVisibility = value;
                OnPropertyChanged(nameof(CancelVisibility));
            }
        }

        #endregion


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
