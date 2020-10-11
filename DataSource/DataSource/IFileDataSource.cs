using DataSource.Models.FileSystem;

namespace DataSource.DataSource
{
    public interface IFileDataSource<TModel, TFile> where TModel : class where TFile : AFileNode
    {
        TModel Read();

        void Write(TModel model);

        void SetFile(TFile fileNode);
    }
}
