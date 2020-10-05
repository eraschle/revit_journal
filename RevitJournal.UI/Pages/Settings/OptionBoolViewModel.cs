using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionBoolViewModel : AOptionViewModel<TaskOption<bool>, bool>
    {
        public OptionBoolViewModel(string name, TaskOption<bool> taskOption) : base(name, taskOption) { }
    }
}
