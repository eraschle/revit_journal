using DataSource.DataSource;
using DataSource.Models;
using DataSource.Models.FileSystem;

namespace DataSource.Metadata
{
    public interface IMetadataDataSource<TModel, TFile> : IFileDataSource<TModel, TFile> 
        where TModel : class where TFile : AFileNode
    {
        MetadataStatus UpdateStatus();
    }
}
