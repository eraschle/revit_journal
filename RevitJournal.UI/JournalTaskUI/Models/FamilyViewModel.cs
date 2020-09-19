using RevitJournalUI.MetadataUI;
using System;
using System.ComponentModel;
using System.Windows.Input;
using DataSource.Metadata;
using DataSource.Model.FileSystem;
using Utilities.UI.Helper;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class FamilyViewModel : PathViewModel
    {
        public RevitFamily RevitFamily { get; private set; }

        public FamilyViewModel(DirectoryViewModel parent, RevitFamily family) : base(parent)
        {
            RevitFamily = family;
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
        }

        public MetadataStatus MetadataStatus
        {
            get { return RevitFamily.MetadataStatus; }
            set { OnPropertyChanged(nameof(MetadataStatus)); }
        }

        public string RevitFileName
        {
            get { return RevitFamily.RevitFile.Name; }
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
    }
}
