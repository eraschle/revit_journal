using RevitJournal.Library.Filtering;
using System.ComponentModel;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    public class CheckedFilterViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal FilterValue FilterValue { get; set; }

        public virtual bool Checked
        {
            get { return FilterValue.IsChecked; }
            set
            {
                if (FilterValue.IsChecked == value) { return; }

                FilterValue.IsChecked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        public string DisplayName
        {
            get { return FilterValue.Name; }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
