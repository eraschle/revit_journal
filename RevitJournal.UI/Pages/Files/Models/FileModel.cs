using RevitJournalUI.MetadataUI;
using System.Windows.Input;
using System;
using DataSource.Models;
using Utilities.System;
using Utilities.UI;
using DataSource.Models.FileSystem;

namespace RevitJournalUI.Pages.Files.Models
{
    public class FileModel : PathModel
    {
        private RevitFamilyFile File
        {
            get { return PathNode as RevitFamilyFile; }
        }

        public FileModel(RevitFamilyFile fileNode) : base(fileNode)
        {
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
        }

        public void AddMetadataEvent()
        {
            File.MetadataUpdated += File_MetadataUpdated;
        }

        private void File_MetadataUpdated(object sender, EventArgs args)
        {
            File.MetadataUpdated -= File_MetadataUpdated;
            UpdateMetadata();
        }

        private void UpdateMetadata()
        {
            NotifyPropertyChanged(nameof(MetadataStatus));
            NotifyPropertyChanged(nameof(LastUpdate));
        }

        public MetadataStatus MetadataStatus
        {
            get { return File.Status; }
        }

        public string LastUpdate
        {
            get
            {
                var metadata = File.Metadata;
                return metadata is object
                    ? DateUtils.AsString(metadata.Updated)
                    : string.Empty;
            }
        }

        public ICommand ViewMetadataCommand { get; }

        private void ViewMetadataCommandAction(object parameter)
        {
            var dialog = new MetadataDialogView(File.Metadata);
            dialog.ShowDialog();
        }
    }
}
