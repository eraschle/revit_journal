using DataSource.Model.Family;
using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public interface IMetadataDataSource
    {
        Family Read();

        void Write(Family model);

        MetadataStatus UpdateStatus();

        void SetFamilyFile(RevitFamilyFile fileNode);
    }
}
