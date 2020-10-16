using DataSource.Models.FileSystem;
using RevitAction.Action;
using RevitJournal.Revit.Filtering;
using RevitJournal.Tasks.Actions;
using RevitJournal.Tasks.Options;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using RevitJournalUI.Pages.Files.Models;
using RevitJournalUI.Pages.Settings;
using RevitJournalUI.Tasks.Actions;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Utilities.UI;

namespace RevitJournalUI.Pages.Files
{
    public class TaskFilesPageModel : APageModel
    {
        private TaskOptions options = TaskOptions.Instance;

        public TaskFilesPageModel()
        {
            FilterCommand = new RelayCommand<object>(FilterCommandAction);
            ActionsCommand = new RelayCommand<object>(ActionsCommandAction, ActionsCommandPredicate);
        }

        public override void SetModelData(object data)
        {
            if (data is null || !(data is SettingsPageModel model)) { return; }

            PathModels.Clear();
            var rootNode = model.FamilyDirectory.Option.GetRootNode<RevitFamilyFile>();
            var rootModel = GetModel(rootNode);
            rootModel.Parent = null;
            PathModels.Add(rootModel);
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
            var dialog = new RevitFamilyFilterView();
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
    }
}
