using DataSource.Models.FileSystem;

namespace RevitJournal.Library
{
    public class LibraryRoot : LibraryFolder
    {
        public int ValidMetadataCount { get; private set; } = 0;

        public int EditMetadataCount { get; private set; } = 0;

        public LibraryRoot(DirectoryNode directory) : base(directory, null)
        {
            AddCountAction(ValidResetAction, ValidCountAction);
            AddCountAction(EditResetAction, EditCountAction);
        }

        private void ValidResetAction(object handler)
        {
            ValidMetadataCount = 0;
        }

        private void ValidCountAction(LibraryFile handler)
        {
            if (handler is null || handler.File.AreMetadataValid == false) { return; }

            ValidMetadataCount += 1;
        }

        private void EditResetAction(object handler)
        {
            EditMetadataCount = 0;
        }

        private void EditCountAction(LibraryFile handler)
        {
            if (handler is null || handler.File.HasFileMetadata == false) { return; }

            EditMetadataCount += 1;
        }
    }
}
