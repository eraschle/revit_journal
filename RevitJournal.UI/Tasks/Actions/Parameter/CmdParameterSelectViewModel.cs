using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    public class CmdParameterSelectViewModel : CmdParameterStringViewModel
    {
        public ObservableCollection<string> ParameterValues { get; } = new ObservableCollection<string>();

        public void UpdateSelectableValues(IList<string> values)
        {
            if (values is null) { return; }
            
            ParameterValues.Clear();
            foreach (var value in values)
            {
                ParameterValues.Add(value);
            }
        }

    }
}
