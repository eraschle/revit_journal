using DataSource.Models;
using System;

namespace DataSource.DataSource
{
    public interface IMetadataContainer<TModel>
    {
        TModel Metadata { get; }

        MetadataStatus Status { get; }

        bool AreMetadataValid { get; }

        bool AreMetadataRepairable { get; }

        bool HasFileMetadata { get; }

        bool HasEditMetadata { get; }
        
        void Update();

        void Write(TModel model);

        void SetApplicationDataSource();

        void SetExternalDataSource();
        
        void SetExternalEditDataSource();
    }
}
