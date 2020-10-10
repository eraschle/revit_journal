using RevitJournal.Tasks.Options.Parameter;
using System.Collections.ObjectModel;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionSelectViewModel<TValue> : AOptionViewModel<TaskOptionSelect<TValue>, TValue>
    {
        public OptionSelectViewModel(string name, TaskOptionSelect<TValue> taskOption, bool showDefaultAtStart)
            : base(name, taskOption, showDefaultAtStart)
        {
            Values = new ObservableCollection<TValue>(Option.Values);
        }

        public ObservableCollection<TValue> Values { get; }
    }
}
