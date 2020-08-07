using System.ComponentModel;

namespace RevitJournalUI.Models
{
    public abstract class ACheckedViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _Checked = false;
        public virtual bool Checked
        {
            get { return _Checked; }
            set
            {
                if(_Checked == value) { return; }

                _Checked = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
