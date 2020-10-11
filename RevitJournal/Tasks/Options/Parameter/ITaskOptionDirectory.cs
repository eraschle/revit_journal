using DataSource.Models.FileSystem;

namespace RevitJournal.Tasks.Options.Parameter
{
    public interface ITaskOptionDirectory: ITaskOption<string>
    {
        DirectoryRootNode GetRootNode<TFile>() where TFile : AFileNode, new();
    }
}
