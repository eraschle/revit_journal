using RevitJournal.Tasks.Options.Parameter;
using System.ComponentModel;
using Utilities.System;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionSliderViewModel : AOptionViewModel<TaskOptionRange, double>
    {
        private readonly string nameSuffix;
        public OptionSliderViewModel(string name, TaskOptionRange taskOption, bool showDefaultAtStart, string labelSuffix = "")
            : base(name, taskOption, showDefaultAtStart)
        {
            nameSuffix = labelSuffix;
            PropertyChanged += OptionSliderViewModel_PropertyChanged;
        }

        private void OptionSliderViewModel_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (StringUtils.Equals(args.PropertyName, nameof(Value)) == false) { return; }

            SliderValue = string.Empty;
        }

        public string SliderValue
        {
            get
            {
                var suffix = string.IsNullOrEmpty(nameSuffix) ? nameSuffix : $" {nameSuffix}";
                return $"({Value}{suffix})";
            }
            set { NotifyPropertyChanged(); }
        }

        public double MinValue
        {
            get { return Option.MinValue; }
        }

        public double MaxValue
        {
            get { return Option.MaxValue; }
        }
    }
}
