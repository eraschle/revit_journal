using System;
using DataSource.Models;
using DataSource.Models.FileSystem;
using DataSource.Model.Metadata;

namespace RevitJournalUI.Pages.Files.Models
{
    public class FileModel : PathModel
    {
        private RevitFamilyFile FileNode
        {
            get { return PathNode as RevitFamilyFile; }
        }

        public FileModel(RevitFamilyFile fileNode) : base(fileNode) { }

        public void AddMetadataEvent()
        {
            FileNode.MetadataUpdated += File_MetadataUpdated;
        }

        private void File_MetadataUpdated(object sender, EventArgs args)
        {
            FileNode.MetadataUpdated -= File_MetadataUpdated;
            MetadataStatus = FileNode.Status;
            CalculateFilesCount();
        }

        public MetadataStatus MetadataStatus
        {
            get { return FileNode.Status; }
            set { NotifyPropertyChanged(); }
        }

        internal override int FileCount
        {
            get { return FileNode is object ? 1 : 0; }
            set { }
        }

        internal override int ValidFileCount
        {
            get { return FileNode is object && FileNode.Status == MetadataStatus.Valid ? 1 : 0; }
            set { NotifyPropertyChanged(); }
        }

        internal override int CheckedFileCount
        {
            get { return IsChecked == true ? 1 : 0; }
            set { NotifyPropertyChanged(); }
        }

        public Family GetMetadata()
        {
            return FileNode?.Metadata;
        }

        protected override void UpdateChildren() { }
    }
}
