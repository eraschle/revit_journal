using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public abstract class AMetadataDataSource<TModel> : IMetadataDataSource<TModel>
    {
        protected RevitFile RevitFile { get; set; }

        protected AMetadataDataSource(RevitFile revitFile)
        {
            RevitFile = revitFile;
        }

        public MetadataStatus Status { get; protected set; } = MetadataStatus.Initial;

        public virtual bool Exist { get { return RevitFile.Exist; } }

        public abstract TModel Read(AFile source = null);

        public abstract void AddFileNameSuffix(params string[] suffixes);

        public abstract void Write(TModel model, AFile destination = null);

        public abstract void UpdateStatus();
    }
}
