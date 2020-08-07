using RevitJournal.Journal.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.Parameters
{
    public class JournalTaskCommandViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IJournalCommand Command { get; set; }

        public string CommandName { get { return Command.Name; } }

        private Visibility _ParameterVisibility = Visibility.Collapsed;
        public Visibility ParameterVisibility
        {
            get { return _ParameterVisibility; }
            set
            {
                if (_ParameterVisibility == value) { return; }

                _ParameterVisibility = value;
                OnPropertyChanged(nameof(ParameterVisibility));
            }
        }

        public ObservableCollection<AJournalCommandParameterViewModel> Parameters { get; }
            = new ObservableCollection<AJournalCommandParameterViewModel>();

        public void UpdateJournalCommandParameters()
        {
            if (Command.HasParameters == false) { return; }

            Parameters.Clear();
            foreach (var parameter in Command.Parameters)
            {
                AJournalCommandParameterViewModel viewModel = null;
                switch (parameter.ParameterType)
                {
                    case JournalParameterType.String:
                        {
                            viewModel = new JournalCommandParameterStringViewModel { CommandParameter = parameter};
                            break;
                        }
                    case JournalParameterType.File:
                        {
                            viewModel = new JournalCommandParameterFileViewModel { CommandParameter = parameter};
                            break;
                        }
                    case JournalParameterType.Boolean:
                        {
                            viewModel = new JournalCommandParameterBooleanViewModel { CommandParameter = parameter};
                            break;
                        }
                }
                if(viewModel is null) { continue; }

                Parameters.Add(viewModel);
            }
        }

        private bool _Checked = false;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked == value) { return; }

                _Checked = value;
                OnPropertyChanged(nameof(Checked));
                SetParameterVisibility();
            }
        }

        private bool _Enabled = true;
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) { return; }

                _Enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        private void SetParameterVisibility()
        {
            ParameterVisibility = Command.HasParameters && Checked ? Visibility.Visible : Visibility.Collapsed;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
