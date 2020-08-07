using System;
using System.ComponentModel;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class CmdParameterInfoViewModel : ACmdParameterViewModel
    {
        public override string ParameterValue
        {
            get { return base.ParameterValue; }
            set
            {
                CommandParameter.Value = value;
                OnPropertyChanged(nameof(ParameterValue));
            }
        }

        public void OnOtherParameterChanged(object sender, PropertyChangedEventArgs arg)
        {
            if (arg is null || arg.PropertyName.Equals(nameof(ParameterValue), StringComparison.CurrentCulture) == false) { return; }

            ParameterValue = string.Empty;
        }
    }
}
