using RevitJournal.Tasks.Options.Parameter;
using System.ComponentModel;
using System.Windows.Input;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionDirectoryViewModel : AOptionViewModel<ITaskOption<string>, string>
    {
        private const string TitleChooseDir = "Choose Directory";

        private readonly ITaskOption<string> otherOption;
        private readonly BackgroundWorker worker;

        public OptionDirectoryViewModel(string name, ITaskOption<string> taskOption, bool showDefaultAtStart, ITaskOption<string> other = null, BackgroundWorker backgroundWorker = null)
            : base(name, taskOption, showDefaultAtStart)
        {
            otherOption = other;
            worker = backgroundWorker;
            SelectCommand = new RelayCommand<object>(SelectAction, SelectPredicate);
        }

        public ICommand SelectCommand { get; }

        private bool SelectPredicate(object parameter)
        {
            return Option is object;
        }

        private void SelectAction(object parameter)
        {
            if (worker is object && worker.IsBusy)
            {
                worker.CancelAsync();
            }
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
            if (worker is object && StringUtils.Equals(Value, value) == false)
            {
                worker.RunWorkerAsync();
            }
        }
    }
}
