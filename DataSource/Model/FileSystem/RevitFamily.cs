using DataSource.Helper;
using DataSource.Metadata;
using DS = DataSource.Model.Family;

namespace DataSource.Model.FileSystem
{
    public class RevitFamily
    {
        private readonly MetadataFamilyJsonContainer MetaDataContainer;
        public const string EditedSuffix = "edited";

        public JsonFile EditedMetadataFile { get; }

        public RevitFamilyFile RevitFile { get; private set; }

        public RevitFamily(RevitFamilyFile revitFile, string libraryPath)
        {
            RevitFile = revitFile;
            var editedFileName = string.Join(Constant.Underline, revitFile.Name, EditedSuffix);
            EditedMetadataFile = revitFile.ChangeExtension<JsonFile>(JsonFile.FileExtension)
                                          .ChangeFileName<JsonFile>(editedFileName);
            MetaDataContainer = new MetadataFamilyJsonContainer(revitFile)
            {
                LibraryPath = libraryPath
            };
        }

        public string LibraryPath
        {
            get { return MetaDataContainer.LibraryPath; }
        }

        public bool HasValidMetadata
        {
            get { return MetaDataContainer.MetadataStatus == MetadataStatus.Valid; }
        }

        public bool HasRepairableMetadata
        {
            get { return HasValidMetadata 
                    || MetaDataContainer.MetadataStatus == MetadataStatus.Repairable; }
        }

        public DS.Family Metadata
        {
            get { return MetaDataContainer.Metadata; }
        }

        public MetadataStatus MetadataStatus
        {
            get { return MetaDataContainer.MetadataStatus; }
        }

        public bool HasFileMetadata
        {
            get { return MetaDataContainer.HasFileMetadata; }
        }

        public DS.Family ReloadedFileMetadata
        {
            get { return MetaDataContainer.ReloadedFileMetadata; }
        }

        public void UpdateStatus(bool reload = false)
        {
            MetaDataContainer.UpdateStatus(reload);
        }

        public void WriteMetaData()
        {
            if (HasValidMetadata == false) { return; }

            MetaDataContainer.WriteMetaData();
        }

        public void WriteEditedMetaData(DS.Family editedMetadata)
        {
            MetaDataContainer.WriteMetaData(editedMetadata, EditedMetadataFile);
        }

        public DS.Family ReadEditedMetaData()
        {
            return MetaDataContainer.ReadMetaData(EditedMetadataFile);
        }
    }
}
