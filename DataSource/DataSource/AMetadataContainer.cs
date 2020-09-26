using DataSource.DataSource.Json;
using DataSource.Metadata;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using DataSource.Xml;

namespace DataSource.DataSource
{
    public abstract class AMetadataContainer
    {
        protected IMetadataDataSource DataSource { get; set; }

        public MetadataStatus MetadataStatus { get; private set; }

        public Family Metadata { get; private set; }

        public bool HasValidMetadata
        {
            get { return MetadataStatus == MetadataStatus.Valid; }
        }

        public bool AreMetadataRepairable
        {
            get { return MetadataStatus == MetadataStatus.Repairable; }
        }

        public void Update()
        {
            MetadataStatus = DataSource.UpdateStatus();
            Metadata = new Family();
            if (MetadataStatus == MetadataStatus.Valid)
            {
                Metadata = DataSource.Read();
            }
        }

        public void Read()
        {
            Update();
        }

        public void Write(Family model)
        {
            DataSource.Write(model);
        }

        public void SetRevitDataSource()
        {
            SetDataSource(new FamilyXmlDataSource());
        }

        public void SetExternalDataSource()
        {
            SetDataSource(new FamilyJsonDataSource());
        }

        public void SetExternalEditDataSource()
        {
            SetDataSource(new FamilyEditJsonDataSource());
        }

        private void SetDataSource(IMetadataDataSource dataSource)
        {
            dataSource.SetFamilyFile(GetFamilyFile());
            DataSource = dataSource;
        }

        public bool HasFileMetadata
        {
            get { return GetJsonFile().Exists(); }
        }

        public bool HasEditMetadata
        {
            get
            {
                var jsonFile = GetJsonFile();
                jsonFile.AddSuffixes(FamilyEditJsonDataSource.Suffix);
                return jsonFile.Exists();
            }
        }

        private JsonFile GetJsonFile()
        {
            return GetFamilyFile().ChangeExtension<JsonFile>();
        }

        protected abstract RevitFamilyFile GetFamilyFile();
    }
}
