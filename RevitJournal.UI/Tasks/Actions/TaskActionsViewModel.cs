using RevitAction.Action;
using RevitJournal.Revit.Journal.Command;
using RevitJournal.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Utilities;

namespace RevitJournalUI.Tasks.Actions
{
    public class TaskActionsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ActionViewModel> ActionViewModels { get; }
            = new ObservableCollection<ActionViewModel>();

        public bool HasCheckedCommands { get { return CheckedActions.Any(); } }

        public IEnumerable<ITaskAction> CheckedActions
        {
            get
            {
                return ActionViewModels
                    .Where(cmd => cmd.Checked)
                    .Select(cmd => cmd.Action);
            }
        }

        public void UpdateAction(TaskManager manager)
        {
            if (manager is null) { return; }

            ActionViewModels.Clear();
            foreach (var command in manager.AllTaskActions)
            {
                var model = new ActionViewModel
                {
                    Action = command
                };
                model.PropertyChanged += new PropertyChangedEventHandler(OnActionChecked);
                model.UpdateJournalCommandParameters();

                if (command is DocumentOpenAction)
                {
                    model.Checked = true;
                    model.Enabled = false;
                }
                ActionViewModels.Add(model);
            }
        }

        private void OnActionChecked(object sender, PropertyChangedEventArgs args)
        {
            ActionViewModel model;
            if (StringUtils.Equals(args.PropertyName, nameof(model.Checked)) == false
                || !(sender is ActionViewModel actionModel)
                || actionModel.Action.MakeChanges == false) { return; }

            var checkChange = AreChangeActionsChecked();
            var checkSave = IsSaveActionChecked();
            if (checkChange && checkSave)
            {
                return;
            }
            else if (checkChange == false && checkSave)
            {
                foreach (var action in GetSaveActions())
                {
                    action.Checked = false;
                }
            }
            else
            {
                var taskAction = GetSaveAction();
                taskAction.Checked = checkChange;
                //taskAction.Enabled = true;
            }
        }

        private bool AreChangeActionsChecked()
        {
            return ActionViewModels.Any(action => action.Action.MakeChanges && action.Checked);
        }

        private bool IsSaveActionChecked()
        {
            return ActionViewModels.Any(action => action.Action.IsSaveAction && action.Checked);
        }

        private ActionViewModel GetSaveAction()
        {
            return ActionViewModels.FirstOrDefault(action => action.Action is DocumentSaveAction);
        }

        private IEnumerable<ActionViewModel> GetSaveActions()
        {
            return ActionViewModels.Where(action => action.Action.IsSaveAction);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
