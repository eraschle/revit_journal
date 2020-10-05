using System.Windows.Input;
using Utilities.UI;
using RevitJournal.Tasks.Options.Parameter;
using Utilities.System;

namespace RevitJournalUI.Pages.Settings
{
    public abstract class AOptionViewModel<TOption, TValue> : ANotifyPropertyChangedModel where TOption : TaskOption<TValue>
    {
        protected TOption Option { get; }

        protected AOptionViewModel(string name, TOption taskOption)
        {
            labelName = name;
            Option = taskOption;
            DefaultCommand = new RelayCommand<object>(DefaultAction, DefaultPredicate);
        }

        private string labelName = string.Empty;
        public virtual string LabelName
        {
            get { return labelName; }
            set
            {
                if (StringUtils.Equals(labelName, value)) { return; }

                labelName = value;
                NotifyPropertyChanged();
            }
        }

        public TValue Value
        {
            get { return Option.Value; }
            set
            {
                if (Option.HasValue(out var optionValue) && optionValue.Equals(value)) { return; }

                Option.Value = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand DefaultCommand { get; }

        private bool DefaultPredicate(object parameter)
        {
            return Option is object;
        }

        private void DefaultAction(object parameter)
        {
            Value = Option.DefaultValue;
        }
    }
}
