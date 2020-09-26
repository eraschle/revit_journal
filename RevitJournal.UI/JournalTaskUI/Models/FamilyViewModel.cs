using RevitJournalUI.MetadataUI;
using System.Windows.Input;
using RevitJournal.Library;
using DataSource.Helper;
using System;
using DataSource.Model;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class FamilyViewModel : PathViewModel<LibraryFile>
    {
        public FamilyViewModel(LibraryFile fileHandler, DirectoryViewModel parent) : base(fileHandler, parent)
        {
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
            Handler.File.MetadataUpdated += File_MetadataUpdated;
        }

        ~FamilyViewModel()
        {
            Handler.File.MetadataUpdated -= File_MetadataUpdated;
        }

        private void File_MetadataUpdated(object sender, EventArgs args)
        {
            UpdateMetadata();
        }

        public MetadataStatus MetadataStatus
        {
            get { return Handler.File.MetadataStatus; }
        }

        public string RevitFileName
        {
            get { return Handler.File.RevitFile.NameWithoutExtension; }
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

        public string LastUpdate
        {
            get
            {
                var metadata = Handler.File.Metadata;
                return metadata is object
                    ? DateUtils.AsString(metadata.Updated)
                    : string.Empty;
            }
        }

        private void UpdateMetadata()
        {
            OnPropertyChanged(nameof(MetadataStatus));
            OnPropertyChanged(nameof(LastUpdate));
        }

        public ICommand ViewMetadataCommand { get; }

        private void ViewMetadataCommandAction(object parameter)
        {
            var dialog = new MetadataDialogView(Handler.File.Metadata);
            dialog.ShowDialog();
        }


    }
}
