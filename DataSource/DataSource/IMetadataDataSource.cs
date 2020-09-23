using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public interface IMetadataDataSource<TModel>
    {
        MetadataStatus Status { get; }

        bool Exist { get; }

        TModel Read(AFileNode source = null);

        void AddFileNameSuffix(params string[] suffixes);

        void Write(TModel model, AFileNode destination = null);

        void UpdateStatus();
    }
}
