using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionInformationViewModel : OptionStringViewModel
    {
        public OptionInformationViewModel(string name, TaskOption<string> taskOption, bool showDefaultAtStart)
            : base(name, taskOption, showDefaultAtStart) { }


        public string InformationValue
        {
            get { return Option.Value; }
            set { NotifyPropertyChanged(); }
        }
    }
}
