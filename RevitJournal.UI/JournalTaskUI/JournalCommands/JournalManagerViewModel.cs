using RevitJournal.Journal;
using RevitJournal.Journal.Command;
using RevitJournal.Journal.Command.Document;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class JournalManagerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<JournalCommandViewModel> CommandViewModels { get; }
            = new ObservableCollection<JournalCommandViewModel>();

        public bool HasCheckedCommands { get { return CheckedCommands.Any(); } }

        public IEnumerable<IJournalCommand> CheckedCommands
        {
            get
            {
                return CommandViewModels
                    .Where(cmd => cmd.Checked)
                    .Select(cmd => cmd.Command);
            }
        }

        public void UpdateCommands(TaskManager manager)
        {
            if (manager is null) { return; }

            CommandViewModels.Clear();
            foreach (var command in manager.UserJournalCommands)
            {
                var model = new JournalCommandViewModel
                {
                    Command = command
                };
                model.PropertyChanged += new PropertyChangedEventHandler(OnJournalCommandChecked);
                model.UpdateJournalCommandParameters();

                if (command is OpenJournalCommand)
                {
                    model.Checked = true;
                    model.Enabled = false;
                }
                CommandViewModels.Add(model);
            }
        }

        private void OnJournalCommandChecked(object sender, PropertyChangedEventArgs args)
        {
            JournalCommandViewModel model;
            if (args.PropertyName.Equals(nameof(model.Checked), StringComparison.CurrentCulture) == false) { return; }
            if (!(sender is JournalCommandViewModel commandModel)) { return; }

            var command = commandModel.Command;
            foreach (var commandViewModel in CommandViewModels)
            {
                var otherCommand = commandViewModel.Command;
                if (command.DependsOnCommand(otherCommand) == false) { continue; }

                commandViewModel.Checked = commandModel.Checked || OtherDependsOnCommand(otherCommand);
                commandViewModel.Enabled = OtherDependsOnCommand(otherCommand) == false;
            }
        }

        private bool OtherDependsOnCommand(IJournalCommand command)
        {
            return CheckedCommands.Any(cmd => cmd.DependsOnCommand(command));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
