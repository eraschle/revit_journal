using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class ParameterViewModel : ANotifyPropertyChangedModel
    {
        private bool isChecked = false;
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                if(isChecked == value) { return; }

                isChecked = value;
                NotifyPropertyChanged();
            }
        }

        public string ParameterName { get; set; }
    }
}
