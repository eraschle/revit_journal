namespace DataSource.Metadata
{
    public class MetadataContainer<TModel> where TModel : class, new()
    {
        protected IMetadataDataSource<TModel> DataSource { get; }

        public MetadataStatus Status
        {
            get { return DataSource.Status; }
        }

        public TModel Metadata { get; private set; }

        public MetadataContainer(IMetadataDataSource<TModel> dataSource)
        {
            DataSource = dataSource;
        }

        public void UpdateStatus(bool reload)
        {
            if (reload == true || Status == MetadataStatus.Initial)
            {
                DataSource.UpdateStatus();
                if (Status == MetadataStatus.Valid)
                {
                    Metadata = DataSource.Read();
                }
                else
                {
                    Metadata = new TModel();
                }
            }
        }
    }
}
