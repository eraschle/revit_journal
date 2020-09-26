using DataSource.Metadata;
using DataSource.Model.Family;

namespace DataSource.DataSource
{
    public interface IMetadataContainer
    {
        void SetDataSource(IMetadataDataSource dataSource);

        Family Metadata { get; }

        MetadataStatus Status { get; }

        bool AreMetadataValid { get; }

        bool AreMetadataRepairable { get; }

        void Update();

        void Write(Family model);
    }
}
