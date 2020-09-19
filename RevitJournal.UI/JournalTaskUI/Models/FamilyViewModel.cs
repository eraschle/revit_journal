using RevitJournalUI.MetadataUI;
using System;
using System.ComponentModel;
using System.Windows.Input;
using DataSource.Metadata;
using Utilities.UI.Helper;
using RevitJournal.Library;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class FamilyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SelectFileHandler FileHandler { get; private set; }

        public FamilyViewModel(SelectFileHandler fileHandler)
        {
            FileHandler = fileHandler;
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
        }

        public MetadataStatus MetadataStatus
        {
            get { return FileHandler.File.MetadataStatus; }
            set { OnPropertyChanged(nameof(MetadataStatus)); }
        }

        public string RevitFileName
        {
            get { return FileHandler.File.RevitFile.Name; }
        }

        public bool Checked
        {
            get { return FileHandler.IsSelected; }
            set
            {
                if (FileHandler.IsSelected == value) { return; }

                FileHandler.IsSelected = value;
                OnPropertyChanged(nameof(Checked));
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
            var dialog = new MetadataDialogView(FileHandler.File.Metadata);
            dialog.ShowDialog();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
