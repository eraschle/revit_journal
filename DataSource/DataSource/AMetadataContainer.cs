using DataSource.Metadata;
using DataSource.Models;
using DataSource.Models.FileSystem;
using System;
using DataSource.Model.Metadata;

namespace DataSource.DataSource
{
    public abstract class AMetadataContainer<TModel, TFile> : IMetadataContainer<TModel> where TModel : class, new() where TFile : AFileNode
    {
      
        protected IMetadataDataSource<TModel, TFile> DataSource { get; set; }

        public MetadataStatus Status { get; private set; }

        public TModel Metadata { get; private set; }

        public bool AreMetadataValid
        {
            get { return Status == MetadataStatus.Valid; }
        }

        public bool AreMetadataRepairable
        {
            get { return Status == MetadataStatus.Repairable; }
        }

        public void Update()
        {
            Status = DataSource.UpdateStatus();
            Metadata = new TModel();
            if (Status == MetadataStatus.Valid)
            {
                Metadata = DataSource.Read();
            }
        }

        public void Read()
        {
            Update();
        }

        public void Write(TModel model)
        {
            DataSource.Write(model);
        }

        public abstract void SetApplicationDataSource();

        public abstract void SetExternalDataSource();

        public abstract void SetExternalEditDataSource();

        public abstract bool HasFileMetadata { get; }

        public abstract bool HasEditMetadata { get; }
    }

    public class MetadataEventArgs : EventArgs
    {
        public MetadataStatus Status { get; private set; }

        public Family Metadata { get; private set; }

        public MetadataEventArgs(MetadataStatus status, Family metadata)
        {
            Status = status;
            Metadata = metadata;
        }
    }
}
