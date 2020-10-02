using DataSource;
using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitJournal.Library;
using RevitJournal.Revit.Filtering;
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
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalManagerUI
{
    public class JournalManagerPageModel : ANotifyPropertyChangedModel
    {
        public const string PrefixCreateButton = "Create";
        public const string PrefixDuplicateButton = "Duplicate";
        public const string PrefixEditButton = "Edit";

        public LibraryManager LibraryManager { get; private set; }

        public TaskManager TaskManager { get; private set; }

        public TaskOptions TaskOptions { get; private set; }

        public JournalManagerPageModel()
        {
            LibraryManager = new LibraryManager { PathBuilder = PathFactory.Instance };
            TaskManager = new TaskManager();
            TaskOptions = new TaskOptions(PathFactory.Instance);
            ProductManager.UpdateVersions();
            TaskOptionViewModel = new TaskOptionViewModel { Options = TaskOptions };
            FamiliesViewModel = new FamilyOverviewViewModel { LibraryManager = LibraryManager };

            PropertyChanged += FamiliesViewModel.OnContentDirectoryChanged;

            CreateCommand = new RelayCommand<FamilyOverviewViewModel>(CreateCommandAction, CreateCommandPredicate);
            BackCommand = new RelayCommand<object>(BackCommandAction);
            DuplicateCommand = new RelayCommand<FamilyOverviewViewModel>(DuplicateCommandAction, DuplicateCommandPredicate);
            EditCommand = new RelayCommand<FamilyOverviewViewModel>(EditCommandAction, EditCommandPredicate);
            ExecuteCommand = new RelayCommand<object>(ExecuteCommandAction, ExecuteCommandPredicate);
            CancelCommand = new RelayCommand<object>(CancelCommandAction, CancelCommandPredicate);

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
            FamilyDirectory = @"C:\develop\workspace\revit_journal_test_data\families";
            JournalDirectory = @"C:\develop\workspace\Content\journal";
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
                if (StringUtils.Equals(TaskOptions.RootDirectory, value)) { return; }

                TaskOptions.RootDirectory = value;
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        public ICommand SetupFilterCommand { get; }

        private void SetupFilterCommandAction(object parameter)
        {
            var dialog = new RevitFamilyFilterView();
            if (dialog.ShowDialog() == true)
            {
                UpdateCheckedFilters();
                FamiliesViewModel.FilterUpdated();
            }
        }

        public ObservableCollection<FilterViewModel> FilterViewModels { get; }
            = new ObservableCollection<FilterViewModel>();

        private void UpdateCheckedFilters()
        {
            FilterViewModels.Clear();
            foreach (var rule in RevitFilterManager.Instance.GetCheckedRules())
            {
                foreach (var value in rule.CheckedValues)
                {
                    var model = new FilterViewModel(rule, value);
                    FilterViewModels.Add(model);
                }
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
                NotifyPropertyChanged();
            }
        }

        public ICommand TaskActionsCommand { get; }

        private void TaskActionsCommandAction(object parameter)
        {
            var actionDirectory = PathFactory.Instance.CreateRoot(ActionDirectory);
            var actions = ExternalAction.GetTaskActions(actionDirectory);
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
                NotifyPropertyChanged();
            }
        }

        private string createName = PrefixCreateButton;
        public string CreateName
        {
            get { return createName; }
            set
            {
                if (StringUtils.Equals(createName, value)) { return; }

                createName = value;
                NotifyPropertyChanged();
            }
        }

        internal void OnCheckedChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args is null) { return; }
            if (StringUtils.Equals(args.PropertyName, nameof(DirectoryViewModel.FilesCountValue)) == false
                || LibraryManager.HasRoot(out var rootModel) == false) { return; }

            CreateName = $"{PrefixCreateButton} [{rootModel.CheckedCount}]";
            UpdateDuplicateName();
            UpdateEditName();
        }

        public ICommand CreateCommand { get; private set; }

        private bool CreateCommandPredicate(FamilyOverviewViewModel model)
        {
            return LibraryManager.HasRoot(out var root)
                && root.CheckedCount > 0
                && Actions.Count > 0;
        }

        private void CreateCommandAction(FamilyOverviewViewModel model)
        {
            TaskManager.ClearTasks();
            TaskManager.SetTaskActions(Actions);

            foreach (var family in LibraryManager.GetCheckedFiles())
            {
                if (TaskManager.IsExecutable(family, TaskOptions) == false) { continue; }

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
                NotifyPropertyChanged();
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
                if (StringUtils.Equals( duplicateName, value)) { return; }

                duplicateName = value;
                NotifyPropertyChanged();
            }
        }

        public void UpdateDuplicateName()
        {
            if (LibraryManager.HasRoot(out var root) == false) { return; }

            DuplicateName = $"{PrefixDuplicateButton} [{root.ValidMetadataCount}]";
        }

        public ICommand DuplicateCommand { get; private set; }

        private bool DuplicateCommandPredicate(FamilyOverviewViewModel model)
        {
            return LibraryManager.HasRoot(out var root)
                && root.ValidMetadataCount > 0;
        }

        private void DuplicateCommandAction(FamilyOverviewViewModel model)
        {
            var dialog = new MetadataDuplicateDialog(LibraryManager)
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
                if (StringUtils.Equals(editName, value)) { return; }

                editName = value;
                NotifyPropertyChanged();
            }
        }

        public void UpdateEditName()
        {
            if (LibraryManager.HasRoot(out var root) == false) { return; }

            EditName = $"{PrefixEditButton} [{root.EditMetadataCount}]";
        }

        public ICommand EditCommand { get; private set; }

        private bool EditCommandPredicate(FamilyOverviewViewModel model)
        {
            return LibraryManager.HasRoot(out var root)
                && root.EditMetadataCount > 0;
        }

        private void EditCommandAction(FamilyOverviewViewModel model)
        {
            var validModels = LibraryManager.EditableFiles();
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
                NotifyPropertyChanged();
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

            TaskOptions.TimeDirectory = DateTime.Now;
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
            CancelVisibility = Visibility.Collapsed;
            BackVisibility = Visibility.Visible;
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
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
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
