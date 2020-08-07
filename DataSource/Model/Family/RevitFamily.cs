using DataSource.Metadata;
using DataSource.Model.FileSystem;

namespace DataSource.Model.Family
{
    public class RevitFamily : IMetaDataContainer<Family>
    {
        private readonly FamilyJsonMetaDataContainer MetaDataContainer;

        public RevitFile RevitFile { get; private set; }

        public bool CanCreateFileMetaData { get { return RevitMetadataStatus == MetadataStatus.Valid; } }

        public RevitFamily(RevitFile revitFile, string libraryPath)
        {
            RevitFile = revitFile;
            MetaDataContainer = new FamilyJsonMetaDataContainer(revitFile)
            {
                LibraryPath = libraryPath
            };
        }

        public Family RevitMetadata { get { return MetaDataContainer.RevitMetadata; } }

        public MetadataStatus RevitMetadataStatus { get { return MetaDataContainer.RevitMetadataStatus; } }

        public Family FileMetaData { get { return MetaDataContainer.FileMetaData; } }

        public MetadataStatus FileMetaDataStatus { get { return MetaDataContainer.FileMetaDataStatus; } }

        public bool HasValidMetadata
        {
            get { return RevitMetadataStatus == MetadataStatus.Valid 
                    || FileMetaDataStatus == MetadataStatus.Valid; }
        }

        public bool HasRepairableMetadata
        {
            get { return HasValidMetadata 
                    || RevitMetadataStatus == MetadataStatus.Repairable 
                    || FileMetaDataStatus == MetadataStatus.Repairable; }
        }

        public bool HasFileMetadata()
        {
            return new Family().Equals(FileMetaData) == false;
        }

        public void UpdateStatus(bool reload = false)
        {
            MetaDataContainer.UpdateStatus(reload);
        }

        public void WriteMetaData()
        {
            if (CanCreateFileMetaData == false) { return; }

            MetaDataContainer.WriteMetaData();
        }
    }
}
