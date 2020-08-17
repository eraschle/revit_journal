using RevitAction.Action;
using RevitJournalUI.Tasks.Actions.Parameter;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace RevitJournalUI.Tasks.Actions
{
    public class ActionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ITaskAction Action { get; set; }

        public string Name { get { return Action.Name; } }

        private Visibility parameterVisibility = Visibility.Collapsed;
        public Visibility ParameterVisibility
        {
            get { return parameterVisibility; }
            set
            {
                if (parameterVisibility == value) { return; }

                parameterVisibility = value;
                OnPropertyChanged(nameof(ParameterVisibility));
            }
        }

        public ObservableCollection<IParameterViewModel> Parameters { get; }
            = new ObservableCollection<IParameterViewModel>();

        public virtual void UpdateParameters()
        {
            if (Action.HasParameters == false) { return; }

            Parameters.Clear();
            foreach (var parameter in Action.Parameters)
            {
                var viewModel = ParameterVmBuilder.Build(parameter);

                if (viewModel is null) { continue; }

                Parameters.Add(viewModel);
            }
            ParameterVmBuilder.ConnectInfo(Parameters);
        }

        private bool isChecked = false;
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                if (isChecked == value) { return; }

                isChecked = value;
                OnPropertyChanged(nameof(Checked));
                SetParameterVisibility();
            }
        }

        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) { return; }

                enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        private void SetParameterVisibility()
        {
            ParameterVisibility = Parameters.Count > 0 && Checked ? Visibility.Visible : Visibility.Collapsed;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
