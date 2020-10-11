using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataSource.Models.FileSystem
{
    public class DirectoryNode : APathNode
    {
        protected string DirectoryName { get; set; }

        private readonly List<DirectoryNode> directoryNodes = new List<DirectoryNode>();
        public IList<DirectoryNode> DirectoryNodes
        {
            get { return directoryNodes; }
        }

        private readonly List<AFileNode> fileNodes = new List<AFileNode>();
        public IList<AFileNode> FileNodes
        {
            get { return fileNodes; }
        }


        public override void SetParent(DirectoryNode parent)
        {
            if (parent is null) { return; }

            RemoveParent();
            Parent = parent;
            parent.AddFolder(this);
        }

        public override void RemoveParent()
        {
            if (HasParent(out var parent) == false) { return; }

            parent.RemoveFolder(this);
            Parent = null;
        }

        public void AddFolder(DirectoryNode folder)
        {
            if (folder is null || DirectoryNodes.Contains(folder)) { return; }

            DirectoryNodes.Add(folder);
        }

        public void RemoveFolder(DirectoryNode folder)
        {
            if (folder is null || DirectoryNodes.Contains(folder) == false) { return; }

            DirectoryNodes.Remove(folder);
        }

        public void AddFile(AFileNode file)
        {
            if (file is null || FileNodes.Contains(file)) { return; }

            FileNodes.Add(file);
        }

        public void RemoveFile(AFileNode file)
        {
            if (file is null || FileNodes.Contains(file) == false) { return; }

            FileNodes.Remove(file);
        }

        public IEnumerable<DirectoryNode> GetDirectories<TFile>(bool recursive, FileSearch<TFile> search = null) where TFile : AFileNode, new()
        {
            return DirectoryNodes.Where(dir => dir.HasFiles(recursive, search));
        }

        public bool HasFiles<TFile>(bool recursive, FileSearch<TFile> search = null) where TFile : AFileNode, new()
        {
            var files = GetFiles(recursive, search);
            return files.Any();
        }

        private IEnumerable<TFile> GetFiles<TFile>(FileSearch<TFile> search) where TFile : AFileNode, new()
        {
            var files = FileNodes.OfType<TFile>();
            return search is null ? files : files.Where(file => search.IsFile(file));
        }

        public IEnumerable<TFile> GetFiles<TFile>(bool recursive, FileSearch<TFile> search = null) where TFile : AFileNode, new()
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
            foreach (var folder in node.DirectoryNodes)
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
