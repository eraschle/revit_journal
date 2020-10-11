using RevitJournalUI.MetadataUI;
using System.Windows.Input;
using System;
using DataSource.Models;
using Utilities.System;
using Utilities.UI;
using DataSource.Models.FileSystem;

namespace RevitJournalUI.Pages.Files.Models
{
    public class FileModel : PathModel<RevitFamilyFile>
    {
        public FileModel(RevitFamilyFile fileNode, FolderModel parent) : base(fileNode, parent)
        {
            ViewMetadataCommand = new RelayCommand<object>(ViewMetadataCommandAction);
        }

        public MetadataStatus MetadataStatus
        {
            get { return PathNode.Status; }
        }

        public string LastUpdate
        {
            get
            {
                var metadata = PathNode.Metadata;
                return metadata is object
                    ? DateUtils.AsString(metadata.Updated)
                    : string.Empty;
            }
        }

        public ICommand ViewMetadataCommand { get; }

        private void ViewMetadataCommandAction(object parameter)
        {
            var dialog = new MetadataDialogView(PathNode.Metadata);
            dialog.ShowDialog();
        }
    }
}
