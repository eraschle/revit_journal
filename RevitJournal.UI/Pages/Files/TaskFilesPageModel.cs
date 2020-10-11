using DataSource.Models.FileSystem;
using RevitAction.Action;
using RevitJournal.Revit.Filtering;
using RevitJournal.Tasks.Actions;
using RevitJournal.Tasks.Options;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using RevitJournalUI.Pages.Files.Models;
using RevitJournalUI.Tasks.Actions;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Utilities.UI;

namespace RevitJournalUI.Pages.Files
{
    public class TaskFilesPageModel : ANotifyPropertyChangedModel
    {
        public TaskOptions Options { get; set; } = new TaskOptions(PathFactory.Instance);

        public TaskFilesPageModel()
        {
            FilterCommand = new RelayCommand<object>(FilterCommandAction);
            ActionsCommand = new RelayCommand<object>(ActionsCommandAction, ActionsCommandPredicate);
        }

        public ObservableCollection<PathModel<APathNode>> PathModels { get; }
            = new ObservableCollection<PathModel<APathNode>>();

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
            return string.IsNullOrEmpty(Options.ActionDirectory.Value) == false
                && Directory.Exists(Options.ActionDirectory.Value);
        }

        private void ActionsCommandAction(object parameter)
        {
            var actionDirectory = PathFactory.Instance.CreateRoot(Options.ActionDirectory.Value);
            var actions = ExternalAction.GetTaskActions(actionDirectory);
            var dialog = new TaskActionsView(actions, Options);
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
