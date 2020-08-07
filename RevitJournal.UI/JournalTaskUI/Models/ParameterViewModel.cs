using System.ComponentModel;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class ParameterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _Checked = false;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if(_Checked == value) { return; }

                _Checked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        public string ParameterName { get; set; }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
