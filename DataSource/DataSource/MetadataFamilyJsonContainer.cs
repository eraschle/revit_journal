using DataSource.Json;
using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public class MetadataFamilyJsonContainer : AMetadataFamilyContainer<MetadataJsonDataSource>
    {
        public MetadataFamilyJsonContainer(RevitFamilyFile revitFile)
            : base(revitFile, new MetadataJsonDataSource(revitFile)) { }
    }
}
