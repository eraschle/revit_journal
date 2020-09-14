using RevitAction.Action;
using RevitJournal.Revit.Journal.Command;
using RevitJournal.Tasks.Options;
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

        public IEnumerable<ITaskAction> CheckedActions
        {
            get
            {
                return ActionViewModels
                    .Where(cmd => cmd.Checked)
                    .Select(cmd => cmd.Action);
            }
        }

        internal void UpdateAction(IEnumerable<ITaskAction> taskActions)
        {
            if (taskActions is null) { return; }

            foreach (var action in taskActions)
            {
                var viewModel = ActionViewModels.FirstOrDefault(act => act.Name == action.Name);
                if(viewModel is null) { continue; }

                viewModel.Action = action;
                viewModel.UpdateParameters();
                viewModel.Checked = true;
            }
        }

        public void UpdateAction(IEnumerable<ITaskAction> taskActions, TaskOptions options)
        {
            if (taskActions is null || options is null) { return; }

            ActionViewModels.Clear();
            foreach (var action in taskActions)
            {
                action.SetLibraryRoot(options.RootDirectory);
                var model = new ActionViewModel
                {
                    Action = action
                };
                if (action is DocumentOpenAction)
                {
                    model = new OpenActionViewModel
                    {
                        Action = action
                    };
                }
                model.PropertyChanged += new PropertyChangedEventHandler(OnActionChecked);
                model.UpdateParameters();

                if (action is DocumentOpenAction)
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
            return ActionViewModels.Any(model => IsModelChecked(model));
        }

        private static bool IsModelChecked(ActionViewModel model)
        {
            if(model.Action is DocumentOpenAction)
            {
                return model is object && model.Action.MakeChanges;
            }
            else
            {
                return model is object && model.Action.MakeChanges && model.Checked;
            }
        }

        private bool IsSaveActionChecked()
        {
            return GetSaveActions().Any(model => model.Checked);
        }

        private IEnumerable<ActionViewModel> GetSaveActions()
        {
            return ActionViewModels.Where(model => IsSaveOrSaveAs(model));
        }

        private ActionViewModel GetSaveAction()
        {
            return GetSaveActions().FirstOrDefault(model => IsSave(model));
        }

        private static bool IsSave(ActionViewModel model)
        {
            return model != null && model.Action is DocumentSaveAction;
        }

        private static bool IsSaveOrSaveAs(ActionViewModel model)
        {
            return IsSave(model) || (model != null && model.Action is DocumentSaveAsAction);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
