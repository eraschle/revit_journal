using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utilities.System;
using Utilities.System.FileSystem;

namespace DataSource.Models.FileSystem
{
    public class PathFactory : IPathBuilder
    {
        private static PathFactory instance;
        public static PathFactory Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = new PathFactory();
                }
                return instance;
            }
        }

        private readonly Dictionary<string, DirectoryNode> directories = new Dictionary<string, DirectoryNode>();
        private readonly Dictionary<string, DirectoryRootNode> rootDirectories = new Dictionary<string, DirectoryRootNode>();

        private PathFactory() { }

        #region RootNode

        public bool HasRoot(string path, out DirectoryRootNode rootNode)
        {
            rootNode = null;
            var rootKey = rootDirectories.Keys.FirstOrDefault(key => StringUtils.Starts(path, key));
            if (string.IsNullOrWhiteSpace(rootKey) == false)
            {
                rootNode = rootDirectories[rootKey];
            }
            return rootNode is object;
        }

        public DirectoryRootNode CreateRoot(string path)
        {
            if (HasRoot(path, out var rootNode)) { return rootNode; }

            if (rootDirectories.ContainsKey(path) == false)
            {
                var node = new DirectoryRootNode(path, this);
                rootDirectories.Add(path, node);
            }
            return rootDirectories[path];
        }

        #endregion

        #region DirectoryNode

        public DirectoryNode Create(string path)
        {
            if (IsDirectory(path, out var directory))
            {
                return directory;
            }
            var parent = CreateParent(path);
            return Create(path, parent);
        }

        public bool IsDirectory(string path, out DirectoryNode directory)
        {
            directory = null;
            if (rootDirectories.ContainsKey(path))
            {
                directory = rootDirectories[path];
            }
            else if (directories.ContainsKey(path))
            {
                directory = directories[path];
            }
            return directory is object;
        }

        private DirectoryNode CreateParent(string path)
        {
            if (IsDirectory(path, out var directory))
            {
                return directory;
            }
            var parentPath = GetParentPath(path);
            return Create(parentPath);
        }

        private DirectoryNode Create(string path, DirectoryNode parent)
        {
            if (HasRoot(path, out _) == false) { throw new ArgumentException($"No Root for {path}"); }

            if (directories.ContainsKey(path) == false)
            {
                var nodeName = GetLastNodeName(path);
                var current = CreateNode<DirectoryNode>(nodeName, parent);
                directories.Add(path, current);
            }
            return directories[path];
        }

        #endregion

        #region FileNode

        public TFile Create<TFile>(string path) where TFile : AFileNode, new()
        {
            var parent = CreateParent(path);
            var nodeName = GetLastNodeName(path);
            return CreateNode<TFile>(nodeName, parent);
        }

        public TFile Create<TFile>(string fileName, DirectoryNode directory) where TFile : AFileNode, new()
        {
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }
            if (IsDirectory(directory.FullPath, out _) == false)
            {
                throw new ArgumentException($"Directory not created {directory.FullPath}");
            }
            return CreateNode<TFile>(fileName, directory);
        }

        #endregion

        #region PathNode

        public TFileNode ChangeRoot<TFileNode>(TFileNode fileNode, string newRootPath) where TFileNode : AFileNode, new()
        {
            if (fileNode is null) { throw new ArgumentNullException(nameof(fileNode)); }
            if (string.IsNullOrWhiteSpace(newRootPath)) { throw new ArgumentNullException(nameof(newRootPath)); }

            var newRoot = CreateRoot(newRootPath);
            var directories = fileNode.GetParentNodes(included: true);
            directories.RemoveAt(0);
            directories.Insert(0, newRoot);
            directories = UpdatePathNode(directories);
            return Create<TFileNode>(fileNode.Name, directories.Last());

        }

        private IList<DirectoryNode> UpdatePathNode(IList<DirectoryNode> directories)
        {
            if (directories is null) { throw new ArgumentNullException(nameof(directories)); }

            for (int idx = 1; idx < directories.Count; idx++)
            {
                var parentIdx = idx - 1;
                var parent = directories[parentIdx];
                var parentPath = parent.FullPath;
                parent = parentIdx == 0 ? CreateRoot(parentPath) : CreateParent(parentPath);

                var current = CreateNode<DirectoryNode>(directories[idx].Name, parent);
                directories[idx] = Create(current.FullPath);
            }
            return directories;
        }

        public TFileNode InsertFolder<TFileNode>(TFileNode fileNode, int index, string folder) where TFileNode : AFileNode, new()
        {
            if (fileNode is null) { throw new ArgumentNullException(nameof(fileNode)); }
            if (string.IsNullOrWhiteSpace(folder)) { throw new ArgumentNullException(nameof(folder)); }

            var directories = fileNode.GetParentNodes(included: true);
            directories.Insert(index, CreateNode<DirectoryNode>(folder));
            UpdatePathNode(directories);
            return Create<TFileNode>(fileNode.Name, directories.Last());
        }

        public TFileNode AddFolder<TFileNode>(TFileNode fileNode, string folder) where TFileNode : AFileNode, new()
        {
            if (fileNode is null) { throw new ArgumentNullException(nameof(fileNode)); }
            if (string.IsNullOrWhiteSpace(folder)) { throw new ArgumentNullException(nameof(folder)); }

            var directories = fileNode.GetParentNodes(included: true);
            directories.Add(CreateNode<DirectoryNode>(folder));
            directories = UpdatePathNode(directories);
            return Create<TFileNode>(fileNode.Name, directories.Last());
        }

        public DirectoryNode AddFolder(DirectoryNode directory, string folder)
        {
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }
            if (string.IsNullOrWhiteSpace(folder)) { throw new ArgumentNullException(nameof(folder)); }

            var directories = directory.GetParentNodes(included: true);
            directories.Add(CreateNode<DirectoryNode>(folder));
            return UpdatePathNode(directories).Last();
        }

        private TPathNode CreateNode<TPathNode>(string nodeName, DirectoryNode parent) where TPathNode : APathNode, new()
        {
            if (parent is null) { throw new ArgumentNullException(nameof(parent)); }

            var pathNode = CreateNode<TPathNode>(nodeName);
            pathNode.SetParent(parent);
            return pathNode;
        }

        private TPathNode CreateNode<TPathNode>(string nodeName) where TPathNode : APathNode, new()
        {
            if (string.IsNullOrWhiteSpace(nodeName) || nodeName.Contains(Constant.PathSeparator))
            {
                throw new ArgumentException($"expected name, but get empty name or path: {nodeName}");
            }
            return new TPathNode { Name = nodeName };
        }

        #endregion

        public string GetParentPath(string path)
        {
            return DirUtils.GetParentPath(path);
        }

        public string GetLastNodeName(string path)
        {
            return DirUtils.GetLastPathName(path);
        }
    }

}
