using DataSource.Models.FileSystem;
using RevitAction.Action;
using RevitJournal.Revit.Filtering;
using RevitJournal.Tasks.Actions;
using RevitJournal.Tasks.Options;
using RevitJournalUI.Pages.Files.Filter;
using RevitJournalUI.Pages.Files.Models;
using RevitJournalUI.Tasks.Actions;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.Pages.Files
{
    public class TaskFilesPageModel : APageModel
    {
        private readonly TaskOptions options = TaskOptions.Instance;

        public TaskFilesPageModel()
        {
            FilterCommand = new RelayCommand<object>(FilterCommandAction);
            ActionsCommand = new RelayCommand<object>(ActionsCommandAction, ActionsCommandPredicate);
        }

        public override void SetModelData(object data)
        {
            PathModels.Clear();
            DebugUtils.StartWatch<TaskFilesPageModel>();
            var rootNode = options.GetFamilyRoot();
            var rootModel = GetModel(rootNode);
            rootModel.Parent = null;
            rootModel.IsChecked = true;
            PathModels.Add(rootModel);
            DebugUtils.StopWatch<TaskFilesPageModel>();
        }

        private FolderModel GetModel(DirectoryNode directory)
        {
            var model = new FolderModel(directory);
            foreach (var child in directory.GetDirectories<RevitFamilyFile>(true))
            {
                var childModel = GetModel(child);
                childModel.Parent = model;
                model.Children.Add(childModel);
            }
            foreach (var file in directory.GetFiles<RevitFamilyFile>(false))
            {
                var fileModel = new FileModel(file) { Parent = model };
                fileModel.AddMetadataEvent();
                model.Children.Add(fileModel);
            }
            return model;
        }

        public ObservableCollection<PathModel> PathModels { get; }
            = new ObservableCollection<PathModel>();

        public ObservableCollection<FilterViewModel> Filters { get; }
            = new ObservableCollection<FilterViewModel>();

        public ICommand FilterCommand { get; }

        private void FilterCommandAction(object parameter)
        {
            var dialog = new FileFilterView();
            if (dialog.ShowDialog() == true)
            {
                UpdateCheckedFilters();
            }
        }

        private void UpdateCheckedFilters()
        {
            Filters.Clear();
            foreach (var rule in RevitFilterManager.Instance.GetCheckedRules())
            {
                foreach (var value in rule.CheckedValues)
                {
                    var model = new FilterViewModel(rule, value);
                    Filters.Add(model);
                }
            }
        }

        public ObservableCollection<ITaskAction> Actions { get; }
          = new ObservableCollection<ITaskAction>();

        public ICommand ActionsCommand { get; }


        private bool ActionsCommandPredicate(object parameter)
        {
            return options is object
                && string.IsNullOrEmpty(options.ActionDirectory.Value) == false
                && Directory.Exists(options.ActionDirectory.Value);
        }

        private void ActionsCommandAction(object parameter)
        {
            var actionDirectory = PathFactory.Instance.CreateRoot(options.ActionDirectory.Value);
            var actions = ExternalAction.GetTaskActions(actionDirectory);
            var dialog = new TaskActionsView(actions, options);
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

        public void SetSelectedModel(PathModel model)
        {
            if (!(model is FileModel file))
            {
                Metadata.HideMetadata();
                return;
            }
            Metadata.UpdateFamily(file.GetMetadata());
        }

        public MetadataViewModel Metadata { get; set; } = new MetadataViewModel();
    }
}
