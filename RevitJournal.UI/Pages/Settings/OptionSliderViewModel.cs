using RevitJournal.Tasks.Options.Parameter;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Utilities.System;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionSliderViewModel<TValue> : AOptionRangeViewModel<TaskOptionRange<TValue>, TValue>
    {
        private readonly IValueConverter converter = new OptionSliderConverter();

        public OptionSliderViewModel(string name, TaskOptionRange<TValue> taskOption) : base(name, taskOption)
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
            get { return $"{labelName} [{converter.Convert(Value, typeof(TValue), null, CultureInfo.CurrentCulture)}]"; }
            set { NotifyPropertyChanged(); }
        }
    }
}
