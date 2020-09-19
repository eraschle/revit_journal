using DataSource;
using RevitAction.Action;
using RevitJournal.Tasks;
using RevitJournal.Tasks.Actions;
using RevitJournal.Tasks.Options;
using RevitJournalUI.JournalTaskUI;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using RevitJournalUI.JournalTaskUI.Models;
using RevitJournalUI.JournalTaskUI.Options;
using RevitJournalUI.MetadataUI;
using RevitJournalUI.Tasks.Actions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        public TaskManager TaskManager { get; private set; }

        public TaskOptions TaskOptions { get; private set; }

        public JournalManagerPageModel()
        {
            TaskManager = new TaskManager();
            TaskOptions = new TaskOptions();
            ProductManager.UpdateVersions();
            TaskOptionViewModel = new TaskOptionViewModel { Options = TaskOptions };
            FamiliesViewModel = new FamilyOverviewViewModel { FilterManager = FilterManager };
            PropertyChanged += FamiliesViewModel.OnContentDirectoryChanged;

            CreateCommand = new RelayCommand<FamilyOverviewViewModel>(CreateCommandAction, CreateCommandPredicate);
            BackCommand = new RelayCommand<object>(BackCommandAction);
            DuplicateCommand = new RelayCommand<FamilyOverviewViewModel>(DuplicateCommandAction, DuplicateCommandPredicate);
            EditCommand = new RelayCommand<FamilyOverviewViewModel>(EditCommandAction, EditCommandPredicate);
            ExecuteCommand = new RelayCommand<object>(ExecuteCommandAction, ExecuteCommandPredicate);
            CancelCommand = new RelayCommand<object>(CancelCommandAction, CancelCommandPredicate);

            FamiliesViewModel.PropertyChanged += new PropertyChangedEventHandler(OnCheckedChanged);

            SetupFilterCommand = new RelayCommand<ObservableCollection<DirectoryViewModel>>(SetupFilterCommandAction);
            TaskActionsCommand = new RelayCommand<object>(TaskActionsCommandAction, TaskActionsCommandPredicate);

            var myDocument = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            FamilyDirectory = myDocument;
            ChooseFamilyDirectoryCommand = new RelayCommand<string>(ChooseFamilyDirectoryAction);
            JournalDirectory = myDocument;
            ChooseJournalDirectoryCommand = new RelayCommand<string>(ChooseJournalDirectoryAction);
            ActionDirectory = myDocument;
            ChooseActionDirectoryCommand = new RelayCommand<string>(ChooseActionDirectoryAction);

#if DEBUG
            FamilyDirectory = @"C:\develop\workspace\Content\Blob";
            JournalDirectory = @"C:\develop\workspace\Content\journal";
#endif
            TaskOptionViewModel.SelectedRevitApp = ProductManager.UseMetadata;
        }

        private void JournalManagerPageModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }


        #region Common Settings

        public TaskOptionViewModel TaskOptionViewModel { get; }

        public string FamilyDirectory
        {
            get { return TaskOptions.RootDirectory; }
            set
            {
                if (StringUtils.Equals(TaskOptions.RootDirectory, value)) { return; }

                TaskOptions.RootDirectory = value;
                OnPropertyChanged(nameof(FamilyDirectory));
            }
        }

        public ICommand ChooseFamilyDirectoryCommand { get; }

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

        public ICommand ChooseJournalDirectoryCommand { get; }

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

        public ICommand ChooseActionDirectoryCommand { get; }

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


        private Visibility fileFilterVisibility = Visibility.Collapsed;
        public Visibility FileFilterVisibility
        {
            get { return fileFilterVisibility; }
            set
            {
                if (fileFilterVisibility == value) { return; }

                fileFilterVisibility = value;
                OnPropertyChanged(nameof(FileFilterVisibility));
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

        private Visibility taskActionsVisibility = Visibility.Visible;
        public Visibility TaskActionsVisibility
        {
            get { return taskActionsVisibility; }
            set
            {
                if (taskActionsVisibility == value) { return; }

                taskActionsVisibility = value;
                OnPropertyChanged(nameof(TaskActionsVisibility));
            }
        }

        public ICommand TaskActionsCommand { get; }

        private void TaskActionsCommandAction(object parameter)
        {
            var actions = ExternalAction.GetTaskActions(ActionDirectory);
            var dialog = new TaskActionsView(actions, TaskOptions);
            if (Actions.Count > 0)
            {
                dialog.UpdateCheckedActions(Actions);
            }
            var result = dialog.ShowDialog();
            if (result == true)
            {
                Actions.Clear();
                foreach (var action in dialog.ViewModel.CheckedActions)
                {
                    Actions.Add(action);
                }
            }
        }

        private bool TaskActionsCommandPredicate(object parameter)
        {
            return string.IsNullOrEmpty(TaskOptions.ActionDirectory) == false
                && Directory.Exists(TaskOptions.ActionDirectory);
        }

        public ObservableCollection<ITaskAction> Actions { get; }
            = new ObservableCollection<ITaskAction>();

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
            TaskManager.SetTaskActions(Actions);

            foreach (var family in model.RecursiveRevitFamilies)
            {
                if ((TaskOptions.UseMetadata && TaskManager.IsRevitInstalled(family, out _) == false)
                    || TaskManager.IsExecutable(family, TaskOptions) == false) { continue; }

                var task = new RevitTask(family);
                TaskManager.AddTask(task, TaskOptions);
            }

            TasksViewModel.Update(TaskManager, TaskOptions);

            FamiliesVisibility = Visibility.Collapsed;
            BackVisibility = Visibility.Visible;
            TaskVisibility = Visibility.Visible;
            TaskActionsVisibility = Visibility.Collapsed;
            FileFilterVisibility = Visibility.Collapsed;
            TaskOptionViewModel.OptionsEnabled = false;
        }

        public ICommand BackCommand { get; private set; }

        private void BackCommandAction(object parameter)
        {
            TaskManager.ClearTasks();
            FamiliesVisibility = Visibility.Visible;
            TaskVisibility = Visibility.Collapsed;
            BackVisibility = Visibility.Collapsed;

            TaskActionsVisibility = Visibility.Visible;
            FileFilterVisibility = Visibility.Visible;
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

            TaskManager.StartServer(TaskOptions);
            TasksViewModel.AddEvents();
            using (var cancel = new CancellationTokenSource())
            {
                Cancellation = cancel;
                TaskManager.SetProgress(TasksViewModel.Progress);
                await TaskManager.ExecuteTasks(TaskOptions, Cancellation.Token)
                                 .ConfigureAwait(false);
                Cancellation = null;
            }
            await CleanupEvents().ConfigureAwait(false);
            ExecuteEnable = true;
        }

        private async Task CleanupEvents()
        {
            await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            TasksViewModel.RemoveEvents();
            TaskManager.StopServer();
        }

        private bool ExecuteCommandPredicate(object parameter)
        {
            return TaskManager.HasTasks;
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
