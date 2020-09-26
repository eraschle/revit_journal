using DataSource.DataSource;
using DataSource.Metadata;
using DataSource.Model;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;

namespace DataSource.Xml
{
    public class FamilyXmlDataSource : AFileDataSource<Family, RevitFamilyFile>, IMetadataDataSource
    {
        internal FamilyBuilder Builder { get; set; }

        internal FamilyTypeBuilder TypeBuilder { get; set; }

        internal RevitXmlRepository Repository { get; set; }

        public FamilyXmlDataSource() : base()
        {
            Repository = new RevitXmlRepository();
            Builder = new FamilyBuilder();
            TypeBuilder = new FamilyTypeBuilder();
        }

        public void SetFamilyFile(RevitFamilyFile revitFile)
        {
            SetFile(revitFile);
            Repository.SetRevitFile(revitFile);
        }

        public override Family Read()
        {
            var family = Builder.Build(Repository);
            var familyTypes = TypeBuilder.Build(Repository);
            family.AddFamilyTypes(familyTypes);
            family.LibraryPath = FileNode.RootPath;
            return family;
        }

        public MetadataStatus UpdateStatus()
        {
            try
            {
                Repository.ReadMetaData();
                return MetadataStatus.Valid;
            }
            catch (FamilyXmlReadException exp)
            {
                return exp.IsRepairable
                    ? MetadataStatus.Repairable
                    : MetadataStatus.Error;
            }
        }

        public override void Write(Family model) { }
    }
}
