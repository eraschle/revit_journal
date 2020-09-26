using System.Collections.Generic;

namespace DataSource.Model.FileSystem
{
    public interface IPathBuilder
    {
        string PathSeparator { get; }

        bool HasRoot(string path, out DirectoryRootNode rootNode);

        DirectoryRootNode CreateRoot(string path);

        IEnumerable<TFile> CreateFiles<TFile>(DirectoryRootNode rootNode, string pattern = null) where TFile : AFileNode, new();

        string GetParentPath(string path);

        string GetLastNodeName(string path);

        DirectoryNode CreateWithPath(string path);

        DirectoryNode CreateWithName(string nodeName);

        IList<DirectoryNode> UpdateOrInsert(IList<DirectoryNode> directories);

        void InsertFolder(DirectoryNode directory, DirectoryNode toInsert);

        TFile Create<TFile>(string path) where TFile : AFileNode, new();
    }

}
