using DataSource.Metadata;
using DataSource.Models.FileSystem;
using DataSource.Model.Metadata;
using DataSource.Xml;
using DataSource.DataSource.Json;

namespace DataSource.DataSource
{
    public class FamiliyMetadataContainer : AMetadataContainer<Family, RevitFamilyFile>
    {
        private readonly RevitFamilyFile file;

        public FamiliyMetadataContainer(RevitFamilyFile familyFile)
        {
            file = familyFile;
        }

        public override void SetApplicationDataSource()
        {
            SetDataSource(new FamilyXmlDataSource());
        }

        public override void SetExternalDataSource()
        {
            SetDataSource(new FamilyJsonDataSource());

        }

        public override void SetExternalEditDataSource()
        {
            SetDataSource(new FamilyEditJsonDataSource());
        }

        private void SetDataSource(IMetadataDataSource<Family, RevitFamilyFile> dataSource)
        {
            dataSource.SetFile(file);
            DataSource = dataSource;
        }

        public override bool HasFileMetadata
        {
            get { return GetJsonFile().Exists(); }
        }

        public override bool HasEditMetadata
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
            return file.ChangeExtension<JsonFile>();
        }
    }
}
