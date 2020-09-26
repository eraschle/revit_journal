using DataSource.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using DS = DataSource.Model.Family;

namespace DataSource.Model.FileSystem
{
    public class RevitFamily : IEquatable<RevitFamily>
    {
        private readonly MetadataFamilyJsonContainer MetaDataContainer;

        public JsonFile EditedMetadataFile { get; }

        public RevitFamilyFile RevitFile { get; private set; }

        public RevitFamily(RevitFamilyFile revitFile)
        {
            RevitFile = revitFile ?? throw new ArgumentNullException(nameof(revitFile));

            EditedMetadataFile = revitFile.ChangeExtension<JsonFile>();
            EditedMetadataFile.AddSuffixes("edited");
            MetaDataContainer = new MetadataFamilyJsonContainer(revitFile)
            {
                LibraryPath = revitFile.RootPath
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

        public override bool Equals(object obj)
        {
            return Equals(obj as RevitFamily);
        }

        public bool Equals(RevitFamily other)
        {
            return other != null &&
                   EqualityComparer<RevitFamilyFile>.Default.Equals(RevitFile, other.RevitFile);
        }

        public override int GetHashCode()
        {
            return 1472110217 + EqualityComparer<RevitFamilyFile>.Default.GetHashCode(RevitFile);
        }

        public static bool operator ==(RevitFamily left, RevitFamily right)
        {
            return EqualityComparer<RevitFamily>.Default.Equals(left, right);
        }

        public static bool operator !=(RevitFamily left, RevitFamily right)
        {
            return !(left == right);
        }
    }
}
