using RevitJournal.Tasks.Options.Parameter;
using System.ComponentModel;
using Utilities.System;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionSliderViewModel : AOptionViewModel<TaskOptionRange, double>
    {
        public OptionSliderViewModel(string name, TaskOptionRange taskOption, bool showDefaultAtStart) 
            : base(name, taskOption, showDefaultAtStart)
        {
            labelName = name;
            PropertyChanged += OptionSliderViewModel_PropertyChanged;
        }

        private void OptionSliderViewModel_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(StringUtils.Equals(args.PropertyName, nameof(Value)) == false) { return; }

            LabelName = string.Empty;
        }

        private string labelName;
        public override string LabelName
        {
            get { return $"{labelName} [{(int)Value}]"; }
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
