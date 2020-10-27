using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSource.Models.FileSystem
{
    public class DirectoryNode : APathNode
    {
        protected string DirectoryName { get; set; }

        private readonly List<DirectoryNode> directoryNodes = new List<DirectoryNode>();

        private readonly List<AFileNode> fileNodes = new List<AFileNode>();

        public void AddChild(APathNode childNode)
        {
            if (childNode is null) { return; }

            if (childNode is DirectoryNode directoryNode
                && directoryNodes.Contains(directoryNode) == false)
            {
                directoryNodes.Add(directoryNode);
            }
            else if (childNode is AFileNode fileNode
                && fileNodes.Contains(fileNode) == false)
            {
                fileNodes.Add(fileNode);
            }
        }

        public void RemoveChild(APathNode childNode)
        {
            if (childNode is null) { return; }

            if (childNode is DirectoryNode directoryNode
                && directoryNodes.Contains(directoryNode))
            {
                directoryNodes.Remove(directoryNode);
            }
            else if (childNode is AFileNode fileNode
                && fileNodes.Contains(fileNode))
            {
                fileNodes.Remove(fileNode);
            }
        }

        public IList<DirectoryNode> GetDirectories<TFile>(bool recursive, FileSearch<TFile> search = null) where TFile : AFileNode, new()
        {
            return directoryNodes.Where(dir => dir.HasFiles(recursive, search)).ToList();
        }

        public bool HasFiles<TFile>(bool recursive, FileSearch<TFile> search = null) where TFile : AFileNode, new()
        {
            return GetFiles(recursive, search).Any();
        }

        private IList<TFile> GetFiles<TFile>(FileSearch<TFile> search) where TFile : AFileNode, new()
        {
            var files = fileNodes.OfType<TFile>().ToList();
            return search is null ? files : files.Where(file => search.IsFile(file)).ToList();
        }

        public IList<TFile> GetFiles<TFile>(bool recursive, FileSearch<TFile> search = null) where TFile : AFileNode, new()
        {
            if (recursive)
            {
                var allFiles = new List<TFile>();
                AddRecursiveFiles(ref allFiles, this, search);
                return allFiles;
            }
            return GetFiles(search);
        }

        private void AddRecursiveFiles<TFile>(ref List<TFile> fileNodes, DirectoryNode node, FileSearch<TFile> search = null) where TFile : AFileNode, new()
        {
            fileNodes.AddRange(node.GetFiles(search));
            foreach (var folder in node.directoryNodes)
            {
                AddRecursiveFiles(ref fileNodes, folder, search);
            }
        }

        public override bool Exists()
        {
            return Directory.Exists(FullPath);
        }

        public override void Create()
        {
            if (Exists()) { return; }

            Directory.CreateDirectory(FullPath);
        }

        public override void Delete()
        {
            if (Exists() == false) { return; }

            Directory.Delete(FullPath);
        }

        protected override void SetNodeName(string name)
        {
            DirectoryName = name;
        }

        protected override string GetNodeName()
        {
            return DirectoryName;
        }
    }
}
