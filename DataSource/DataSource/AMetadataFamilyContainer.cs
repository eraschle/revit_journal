using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using DataSource.Xml;

namespace DataSource.Metadata
{
    public abstract class AMetadataFamilyContainer<TFileDataSource>
        : AMetadataContainer<Family, FamilyXmlDataSource, TFileDataSource>
          where TFileDataSource : AMetadataDataSource<Family>
    {
        public string LibraryPath
        {
            get { return RevitDataSource.LibraryPath; }
            set { RevitDataSource.LibraryPath = value; }
        }

        protected AMetadataFamilyContainer(RevitFile revitFile, TFileDataSource fileDataSource)
            : base(new FamilyXmlDataSource(revitFile), fileDataSource) { }
    }
}
