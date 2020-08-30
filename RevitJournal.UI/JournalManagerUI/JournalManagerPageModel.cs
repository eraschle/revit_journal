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
using System.IO;
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

        private readonly Progress<TaskReport> Progress;

        public TaskManager TaskManager { get; private set; }
        
        public TaskOptions TaskOptions { get; private set; }

        public JournalManagerPageModel()
        {
            TaskManager = new TaskManager();
            TaskOptions = new TaskOptions();
            ProductManager.UpdateVersions();
            TaskOptionViewModel = new TaskOptionViewModel { Options = TaskOptions };
            FamiliesViewModel = new FamilyOverviewViewModel { FilterManager = FilterManager };
            PropertyChanged += new PropertyChangedEventHandler(FamiliesViewModel.OnContentDirectoryChanged);
            CreateCommand = new RelayCommand<FamilyOverviewViewModel>(CreateCommandAction, CreateCommandPredicate);
            BackCommand = new RelayCommand<object>(BackCommandAction);
            DuplicateCommand = new RelayCommand<FamilyOverviewViewModel>(DuplicateCommandAction, DuplicateCommandPredicate);
            EditCommand = new RelayCommand<FamilyOverviewViewModel>(EditCommandAction, EditCommandPredicate);
            ExecuteCommand = new RelayCommand<object>(ExecuteCommandAction, ExecuteCommandPredicate);
            CancelCommand = new RelayCommand<object>(CancelCommandAction, CancelCommandPredicate);

            Progress = new Progress<TaskReport>();
            FamiliesViewModel.PropertyChanged += new PropertyChangedEventHandler(OnCheckedChanged);

            SetupFilterCommand = new RelayCommand<ObservableCollection<DirectoryViewModel>>(SetupFilterCommandAction);
            SetupJournalCommand = new RelayCommand<object>(SetupJournalCommandAction, SetupJournalCommandPredicate);

            var myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FamilyDirectory = myDocument;
            JournalDirectory = myDocument;
            ActionDirectory = myDocument;

#if DEBUG
            FamilyDirectory = @"C:\develop\workspace\revit_journal_test_data\families";
            JournalDirectory = @"C:\develop\workspace\revit_journal_test_data\journals";
#endif
            TaskOptionViewModel.SelectedRevitApp = ProductManager.UseMetadata;
        }


        #region Common Settings

        public TaskOptionViewModel TaskOptionViewModel { get; }

        public string FamilyDirectory
        {
            get { return TaskOptions.RootDirectory; }
            set
            {
                if (StringUtils.Equals(TaskOptions.RootDirectory ,value)) { return; }

                TaskOptions.RootDirectory = value;
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
            get { return TaskOptions.JournalDirectory; }
            set
            {
                if (StringUtils.Equals(TaskOptions.JournalDirectory, value)) { return; }

                TaskOptions.JournalDirectory = value;
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

        public string ActionDirectory
        {
            get { return TaskOptions.ActionDirectory; }
            set
            {
                if (StringUtils.Equals(TaskOptions.ActionDirectory, value)) { return; }

                TaskOptions.ActionDirectory = value;
                OnPropertyChanged(nameof(ActionDirectory));
            }
        }

        public ICommand ChooseActionDirectoryCommand
        {
            get { return new RelayCommand<string>(ChooseActionDirectoryAction); }
        }

        private void ChooseActionDirectoryAction(string selectedPath)
        {
            ActionDirectory = ChooseDirectory(selectedPath);
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

        private void SetupJournalCommandAction(object parameter)
        {
            var actions = TaskActionHelper.GetTaskActions(ActionDirectory);
            var dialog = new TaskActionsView(actions);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                UpdateActions(dialog.ViewModel);
            }
        }

        private bool SetupJournalCommandPredicate(object parameter)
        {
            return string.IsNullOrEmpty(TaskOptions.ActionDirectory) == false
                && Directory.Exists(TaskOptions.ActionDirectory);
        }

        public ObservableCollection<ITaskAction> Actions { get; }
            = new ObservableCollection<ITaskAction>();

        private void UpdateActions(TaskActionsViewModel viewModel)
        {
            Actions.Clear();
            foreach (var commands in viewModel.CheckedActions)
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
            UpdateDuplicateName();
            UpdateEditName();
        }

        public ICommand CreateCommand { get; private set; }

        private bool CreateCommandPredicate(FamilyOverviewViewModel model)
        {
            return model != null
                && model.HasCheckedRevitFiles
                && Actions.Count > 0;
        }

        private void CreateCommandAction(FamilyOverviewViewModel model)
        {
            TaskManager.ClearTasks();
            var arguments = TaskOptions.Arguments;
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
                foreach (var action in Actions)
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

        #endregion

        #region Duplicated families

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

        public void UpdateDuplicateName()
        {
            DuplicateName = $"{PrefixDuplicateButton} [{FamiliesViewModel.ValidCheckedRevitFilesCount}]";
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

        #endregion

        #region Edit metadata 

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

        public void UpdateEditName()
        {
            EditName = $"{PrefixEditButton} [{FamiliesViewModel.EditableRevitFilesCount}]";
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

        #region Tasks Overview

        public TaskOverviewViewModel TasksViewModel { get; } = new TaskOverviewViewModel();

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

            Progress.ProgressChanged += new EventHandler<TaskReport>(OnReport);
            using (var cancel = new CancellationTokenSource())
            {
                Cancellation = cancel;
                await TaskManager.ExecuteTasks(TaskOptions, Cancellation.Token)
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

        private void OnReport(object sender, TaskReport result)
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
