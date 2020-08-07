using RevitMetaData.Journal.Command;
using System;
using System.ComponentModel;

namespace RevitMetaDataUI.JournalTaskUI
{
    public class JournalTaskCommandParameterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public IJournalCommandParameter CommandParameter { get; set; }

        public string ParameterName { get { return CommandParameter.Name; } }

        public string ParameterValue
        {
            get { return CommandParameter.Value; }
            set
            {
                if(CommandParameter.Value.Equals(value, StringComparison.CurrentCulture)) { return; }

                CommandParameter.Value = value;
                OnPropertyChanged(nameof(ParameterValue));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
