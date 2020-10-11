using RevitJournal.Tasks.Options.Parameter;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.Pages.Settings.Models
{
    public class OptionDirectoryViewModel : AOptionViewModel<ITaskOptionDirectory, string>
    {
        private const string TitleChooseDir = "Choose Directory";

        private readonly ITaskOption<string> otherOption;

        private readonly BackgroundWorker worker;

        public OptionDirectoryViewModel(string name, ITaskOptionDirectory taskOption, bool showDefaultAtStart, ITaskOptionDirectory other = null, BackgroundWorker backgroundWorker = null)
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
                if (worker.IsBusy)
                {
                    worker.CancelAsync();
                }
                while (worker.CancellationPending)
                {
                    Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
                }
                worker.RunWorkerAsync(Option);
            }
        }
    }
}
