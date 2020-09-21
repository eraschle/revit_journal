using RevitJournalUI.MetadataUI;
using System.Windows.Input;
using DataSource.Metadata;
using Utilities.UI.Helper;
using RevitJournal.Library;
using DataSource.Helper;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class FamilyViewModel : PathViewModel<LibraryFile>
    {
        public FamilyViewModel(LibraryFile fileHandler, DirectoryViewModel parent) : base(fileHandler, parent)
        {
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
        }

        public MetadataStatus MetadataStatus
        {
            get { return Handler.File.MetadataStatus; }
            set { OnPropertyChanged(nameof(MetadataStatus)); }
        }

        public string RevitFileName
        {
            get { return Handler.File.RevitFile.Name; }
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
                    ? DateHelper.AsString(metadata.Updated)
                    : string.Empty;
            }
        }

        public ICommand ViewMetadataCommand { get; }

        private void ViewMetadataCommandAction(object parameter)
        {
            var dialog = new MetadataDialogView(Handler.File.Metadata);
            dialog.ShowDialog();
        }
    }
}
