using System.Collections.Generic;

namespace DataSource.Model.FileSystem
{
    public interface IPathBuilder
    {
        bool HasRoot(string path, out DirectoryRootNode rootNode);

        DirectoryRootNode CreateRoot(string path);

        TFileNode ChangeRoot<TFileNode>(TFileNode fileNode, string newRootPath) where TFileNode : AFileNode, new();

        IEnumerable<TFile> CreateFiles<TFile>(DirectoryRootNode rootNode, string pattern = null) where TFile : AFileNode, new();

        string GetParentPath(string path);

        string GetLastNodeName(string path);

        DirectoryNode Create(string path);

        TFileNode InsertFolder<TFileNode>(TFileNode fileNode, int index, string folder) where TFileNode : AFileNode, new();

        DirectoryNode AddFolder(DirectoryNode directory, string folder);

        TFileNode AddFolder<TFileNode>(TFileNode fileNode, string folder) where TFileNode : AFileNode, new();

        TFile Create<TFile>(string path) where TFile : AFileNode, new();

        TFile Create<TFile>(string fileName, DirectoryNode directory) where TFile : AFileNode, new();
    }

}
