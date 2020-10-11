using RevitJournal.Tasks.Options.Parameter;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionInformationViewModel : OptionStringViewModel
    {
        public OptionInformationViewModel(string name, ITaskOption<string> taskOption, bool showDefaultAtStart)
            : base(name, taskOption, showDefaultAtStart) { }


        public string InformationValue
        {
            get { return Option.Value; }
            set { NotifyPropertyChanged(); }
        }
    }
}
