using RevitJournal.Tasks.Options.Parameter;
using System.Windows.Input;
using Utilities.UI;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionDirectoryViewModel : OptionStringViewModel
    {
        private const string TitleChooseDir = "Choose Directory";

        private readonly TaskOption<string> otherOption;

        public OptionDirectoryViewModel(string name, TaskOption<string> taskOption, TaskOption<string> other = null) : base(name, taskOption)
        {
            otherOption = other;
            SelectCommand = new RelayCommand<object>(SelectAction, SelectPredicate);
        }

        public ICommand SelectCommand { get; }

        private bool SelectPredicate(object parameter)
        {
            return Option is object;
        }

        private void SelectAction(object parameter)
        {
            var value = Option.Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                value = Option.DefaultValue;
            }
            if (otherOption is object && string.IsNullOrWhiteSpace(value))
            {
                value = otherOption.Value;
            }
            Value = PathDialog.ChooseDir(TitleChooseDir, value);
        }
    }
}
