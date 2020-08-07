using RevitJournalUI.Helper;
using RevitJournalUI.MetadataUI;
using System;
using System.ComponentModel;
using System.Windows.Input;
using DataSource.Metadata;
using DataSource.Model.FileSystem;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class FamilyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly DirectoryViewModel Parent;
        
        public RevitFamily RevitFamily { get; private set; }

        public FamilyViewModel(DirectoryViewModel parent, RevitFamily family)
        {
            Parent = parent;
            RevitFamily = family;
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
        }

        public MetadataStatus MetadataStatus
        {
            get { return RevitFamily.MetadataStatus; }
            set { OnPropertyChanged(nameof(MetadataStatus)); }
        }

        public string RevitFileName { get { return RevitFamily.RevitFile.Name; } }

        private bool _Checked = true;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if(_Checked == value) { return; }

                _Checked = value;
                OnPropertyChanged(nameof(Checked));
                Parent.UpdateParent();
            }
        }

        private bool _Enabled = true;
        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value) { return; }

                _Enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        private string _LastUpdate = string.Empty;
        public string LastUpdate
        {
            get { return _LastUpdate; }
            set
            {
                if (_LastUpdate.Equals(value, StringComparison.CurrentCulture)) { return; }

                _LastUpdate = value;
                OnPropertyChanged(nameof(LastUpdate));
            }
        }

        public ICommand ViewMetadataCommand { get; }

        private void ViewMetadataCommandAction(object parameter)
        {
            var dialog = new MetadataDialogView(RevitFamily.Metadata);
            dialog.ShowDialog();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
