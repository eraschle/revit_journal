using DataSource.Helper;
using DataSource.Metadata;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;

namespace DataSource.Xml
{
    public class FamilyXmlDataSource : AMetadataDataSource<Family>
    {

        private readonly FamilyBuilder FamilyBuilder;
        private readonly FamilyTypeBuilder FamilyTypeBuilder;
        private readonly RevitXmlRepository Repository;

        public string LibraryPath { get; set; }

        public FamilyXmlDataSource(RevitFamilyFile revitFile) : base(revitFile)
        {
            Repository = new RevitXmlRepository(revitFile);
            FamilyBuilder = new FamilyBuilder();
            FamilyTypeBuilder = new FamilyTypeBuilder();
        }

        public override Family Read(AFileNode source = null)
        {
            if (source != null && source is RevitFamilyFile rvtSource)
            {
                RevitFile = rvtSource;
                Repository.SetRevitFile(RevitFile);
            }

            var family = FamilyBuilder.Build(Repository);
            if (RevitFile is object)
            {
                family.LibraryPath = RevitFile.RootPath;
            }
            var types = FamilyTypeBuilder.Build(Repository);
            family.AddFamilyTypes(types);
            return family;
        }

        public override void UpdateStatus()
        {
            var status = MetadataStatus.Initial;
            try
            {
                Repository.ReadMetaData();
                status = MetadataStatus.Valid;
            }
            catch (FamilyXmlReadException exp)
            {
                if (exp.IsRepairable)
                {
                    status = MetadataStatus.Repairable;
                }
                else
                {
                    status = MetadataStatus.Error;
                }
            }
            finally
            {
                Status = status;
            }
        }

        public override void Write(Family model, AFileNode destination = null) { }

        public override void AddFileNameSuffix(params string[] suffixes) { }
    }
}
