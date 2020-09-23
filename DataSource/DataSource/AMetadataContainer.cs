using DataSource.Model.FileSystem;

namespace DataSource.Metadata
{
    public abstract class AMetadataContainer<TModel, TRevitDataSource, TFileDataSource>
        : IMetadataContainer<TModel>
        where TModel : class, new()
        where TRevitDataSource : AMetadataDataSource<TModel>
        where TFileDataSource : AMetadataDataSource<TModel>
    {
        protected AMetadataContainer(TRevitDataSource revitDataSource, TFileDataSource fileDataSource)
        {
            RevitDataSource = revitDataSource;
            Revit = new MetadataContainer<TModel>(RevitDataSource);
            FileDataSource = fileDataSource;
            File = new MetadataContainer<TModel>(FileDataSource);
        }

        protected TRevitDataSource RevitDataSource { get; }

        protected MetadataContainer<TModel> Revit { get; set; }

        protected TFileDataSource FileDataSource { get; }

        protected MetadataContainer<TModel> File { get; set; }

        protected TModel RevitMetadata { get { return Revit.Metadata; } }

        protected MetadataStatus RevitMetadataStatus { get { return Revit.Status; } }

        protected TModel FileMetadata { get { return File.Metadata; } }

        protected MetadataStatus FileMetadataStatus { get { return File.Status; } }

        public TModel Metadata
        {
            get
            {
                if (HasFileMetadata)
                {
                    return FileMetadata;
                }
                return RevitMetadata;
            }
        }

        public MetadataStatus MetadataStatus
        {
            get { return RevitMetadataStatus; }
        }

        public bool HasFileMetadata
        {
            get { return FileDataSource.Exist && FileMetadataStatus == MetadataStatus.Valid; }
        }

        public TModel ReloadedFileMetadata { get { return FileDataSource.Read(); } }

        public void UpdateStatus(bool reload = false)
        {
            if (reload == true || RevitMetadataStatus == MetadataStatus.Initial)
            {
                Revit.UpdateStatus(reload);
                File.UpdateStatus(reload);
            }
        }

        public void WriteMetaData(TModel model = null, AFileNode destination = null)
        {
            if (model is null && RevitMetadataStatus == MetadataStatus.Valid)
            {
                model = RevitMetadata;
            }

            if (model is null) { return; }

            if (destination is null)
            {
                FileDataSource.Write(model);
            }
            else
            {
                FileDataSource.Write(model, destination);
            }
        }

        public TModel ReadMetaData(AFileNode source)
        {
            return FileDataSource.Read(source);
        }
    }
}
