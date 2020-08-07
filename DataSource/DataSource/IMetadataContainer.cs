using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public interface IMetadataContainer<TModel> where TModel : class
    {
        TModel Metadata { get; }

        MetadataStatus MetadataStatus { get; }

        void UpdateStatus(bool reload);

        bool HasFileMetadata { get; }

        void WriteMetaData(TModel model = null, AFile destination = null);
    }
}
