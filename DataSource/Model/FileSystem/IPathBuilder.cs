namespace DataSource.Model.FileSystem
{
    public interface IPathBuilder
    {
        DirectoryNode Create(string path);

        TFile Create<TFile>(string path, DirectoryNode directory) where TFile : AFileNode, new();

        string GetLastRemoved(string fullPath, string removeName);

        string GetLast(string fullPath);

    }

}
