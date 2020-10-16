using RevitJournal.Tasks.Options.Parameter;
using System.ComponentModel;
using System.Windows.Input;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionDirectoryViewModel : AOptionViewModel<ITaskOptionDirectory, string>
    {
        private const string TitleChooseDir = "Choose Directory";

        private readonly ITaskOption<string> otherOption;

        public OptionDirectoryViewModel(string name, ITaskOptionDirectory taskOption, bool showDefaultAtStart, ITaskOptionDirectory other = null)
            : base(name, taskOption, showDefaultAtStart)
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
