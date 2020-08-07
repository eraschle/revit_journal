using DataSource.Json;
using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public class MetadataFamilyJsonContainer : AMetadataFamilyContainer<MetadataJsonDataSource>
    {
        public MetadataFamilyJsonContainer(RevitFile revitFile)
            : base(revitFile, new MetadataJsonDataSource(revitFile)) { }
    }
}
