using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public abstract class AMetadataDataSource<TModel> : IMetadataDataSource<TModel>
    {
        protected RevitFamilyFile RevitFile { get; set; }

        protected AMetadataDataSource(RevitFamilyFile revitFile)
        {
            RevitFile = revitFile;
        }

        public MetadataStatus Status { get; protected set; } = MetadataStatus.Initial;

        public virtual bool Exist { get { return RevitFile.Exists(); } }

        public abstract TModel Read(AFileNode source = null);

        public abstract void AddFileNameSuffix(params string[] suffixes);

        public abstract void Write(TModel model, AFileNode destination = null);

        public abstract void UpdateStatus();
    }
}
