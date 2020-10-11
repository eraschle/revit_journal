using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionBoolViewModel : AOptionViewModel<ITaskOption<bool>, bool>
    {
        public OptionBoolViewModel(string name, ITaskOption<bool> taskOption, bool showDefaultAtStart)
            : base(name, taskOption, showDefaultAtStart) { }
    }
}
