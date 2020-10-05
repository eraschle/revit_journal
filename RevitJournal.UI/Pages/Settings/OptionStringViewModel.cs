using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionStringViewModel : AOptionViewModel<TaskOption<string>, string>
    {
        public OptionStringViewModel(string name, TaskOption<string> taskOption) : base(name, taskOption) { }
    }
}
