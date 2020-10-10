using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionStringViewModel : AOptionViewModel<TaskOption<string>, string>
    {
        public OptionStringViewModel(string name, TaskOption<string> taskOption, bool showDefaultAtStart)
            : base(name, taskOption, showDefaultAtStart) { }
    }
}
