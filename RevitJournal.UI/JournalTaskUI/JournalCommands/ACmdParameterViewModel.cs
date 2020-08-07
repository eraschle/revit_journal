using RevitJournal.Journal.Command;
using System;
using System.ComponentModel;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public abstract class ACmdParameterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private ICommandParameter _CommandParameter;
        public ICommandParameter CommandParameter
        {
            get { return _CommandParameter; }
            set
            {
                _CommandParameter = value;
                if (_CommandParameter != null)
                {
                    IsEnable = _CommandParameter.IsEnable;
                }
            }
        }

        public string ParameterName { get { return CommandParameter.Name; } }

        public virtual string ParameterValue
        {
            get { return CommandParameter.Value; }
            set
            {
                if (CommandParameter.Value.Equals(value, StringComparison.CurrentCulture)) { return; }

                CommandParameter.Value = value;
                OnPropertyChanged(nameof(ParameterValue));
            }
        }

        private bool _IsEnable;
        public virtual bool IsEnable
        {
            get { return _IsEnable; }
            set
            {
                if (_IsEnable == value) { return; }

                _IsEnable = value;
                OnPropertyChanged(nameof(IsEnable));
            }
        }

        private Visibility _Visibility = Visibility.Visible;
        public virtual Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (_Visibility == value) { return; }

                _Visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
