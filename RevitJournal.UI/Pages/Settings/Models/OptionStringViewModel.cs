using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionStringViewModel : AOptionViewModel<ITaskOption<string>, string>
    {
        public OptionStringViewModel(string name, ITaskOption<string> taskOption, bool showDefaultAtStart)
            : base(name, taskOption, showDefaultAtStart) { }
    }
}
