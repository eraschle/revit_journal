using DataSource.Model.FileSystem;

namespace RevitJournal.Library
{
    public class LibraryRoot : LibraryFolder
    {
        public int ValidMetadataCount { get; private set; } = 0;

        public int EditMetadataCount { get; private set; } = 0;

        public LibraryRoot(RevitDirectory directory) : base(directory, null) { }

        public override void UpdateFileCounts()
        {
            ValidMetadataCount = 0;
            EditMetadataCount = 0;
            base.UpdateFileCounts();
        }

        protected override void UpdateCountAction(LibraryFile handler)
        {
            base.UpdateCountAction(handler);
            if(handler is null || handler.IsCheckedAndAllowed() == false) { return; }

            if (handler.File.HasValidMetadata)
            {
                ValidMetadataCount += 1;
            }
            if (handler.File.HasFileMetadata)
            {
                EditMetadataCount += 1;
            }
        }
    }
}
