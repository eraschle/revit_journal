using System.Windows.Input;
using Utilities.UI;
using RevitJournal.Tasks.Options.Parameter;
using Utilities.System;
using System.Windows;

namespace RevitJournalUI.Pages.Settings.Models
{
    public abstract class AOptionViewModel<TOption, TValue> : ANotifyPropertyChangedModel
        where TOption : ITaskOption<TValue>
    {
        public TOption Option { get; }

        protected AOptionViewModel(string name, TOption taskOption, bool showDefaultAtStart)
        {
            labelName = name;
            Option = taskOption;
            DefaultCommand = new RelayCommand<object>(DefaultAction, DefaultPredicate);
            if (showDefaultAtStart && Option.HasValue(out _) == false)
            {
                Value = Option.DefaultValue;
            }
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

        private Visibility optionVisibility = Visibility.Visible;
        public Visibility OptionVisibility
        {
            get { return optionVisibility; }
            set
            {
                if (optionVisibility == value) { return; }

                optionVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled == value) { return; }

                isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand DefaultCommand { get; }

        public bool ShowDefaultValue { get; set; }

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
