using RevitJournal.Library.Filtering;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    public class CheckedFilterViewModel : ANotifyPropertyChangedModel
    {
        internal FilterValue FilterValue { get; set; }

        public virtual bool Checked
        {
            get { return FilterValue.IsChecked; }
            set
            {
                if (FilterValue.IsChecked == value) { return; }

                FilterValue.IsChecked = value;
                NotifyPropertyChanged();
            }
        }

        public string DisplayName
        {
            get { return FilterValue.Name; }
        }
    }
}
