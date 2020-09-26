using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Utilities;

namespace DataSource.Model.FileSystem
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

        private static char Separator { get; } = Path.DirectorySeparatorChar;

        public string PathSeparator { get; } = Separator.ToString(CultureInfo.CurrentCulture);

        private readonly Dictionary<string, DirectoryNode> directories = new Dictionary<string, DirectoryNode>();
        private readonly Dictionary<string, DirectoryRootNode> rootDirectories = new Dictionary<string, DirectoryRootNode>();

        private PathFactory() { }

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
            if (HasRoot(path, out var rootNode))
            {
                throw new ArgumentException($"Root with parent folder already exists {rootNode.FullPath}");
            }

            if (rootDirectories.ContainsKey(path) == false)
            {
                var node = new DirectoryRootNode(path, this);
                rootDirectories.Add(path, node);
            }
            return rootDirectories[path];
        }

        public IEnumerable<TFile> CreateFiles<TFile>(DirectoryRootNode rootNode, string pattern = null) where TFile : AFileNode, new()
        {
            if (rootNode is null) { throw new ArgumentNullException(nameof(rootNode)); }

            var search = new TFile().GetSearchPattern(pattern);
            var option = SearchOption.AllDirectories;
            var files = new List<TFile>();
            foreach (var file in Directory.GetFiles(rootNode.FullPath, search, option))
            {
                files.Add(Create<TFile>(file));
            }
            return files;
        }

        public string GetParentPath(string path)
        {
            var lastName = GetLastNodeName(path);
            return path.Remove(path.Length - (lastName.Length + 1));
        }

        public string GetLastNodeName(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(nameof(path)); }

            var split = path.Split(Separator);
            if (split.Length <= 1)
            {
                throw new ArgumentException($"Path not valid: {path}");
            }
            return split[split.Length - 1];
        }

        public DirectoryNode CreateWithPath(string path)
        {
            if (IsDirectory(path, out var directory))
            {
                return directory;
            }
            var parent = CreateParent(path);
            return Create(path, parent);
        }

        public DirectoryNode CreateWithName(string nodeName)
        {
            return CreateNode<DirectoryNode>(nodeName);
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
            return CreateWithPath(parentPath);
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

        public void InsertFolder(DirectoryNode directory, DirectoryNode toInsert)
        {
            if (directory is null || toInsert is null) { return; }

            foreach (var folder in directory.DirectoryNodes)
            {
                var newFolder = CreateWithName(folder.Name);
                newFolder.SetParent(toInsert);
            }
            foreach (var file in directory.FileNodes)
            {
                var newFile = (AFileNode)Activator.CreateInstance(file.GetType());
                newFile.Name = string.Copy(file.Name);
                newFile.SetParent(toInsert);
            }

            toInsert.SetParent(directory);
        }

        public IList<DirectoryNode> UpdateOrInsert(IList<DirectoryNode> directories)
        {
            if (directories is null) { throw new ArgumentNullException(nameof(directories)); }

            for (int idx = 1; idx < directories.Count; idx++)
            {
                var parentIdx = idx - 1;
                var parent = directories[parentIdx];
                var parentPath = parent.FullPath;
                parent = parentIdx == 0 ? CreateRoot(parentPath) : CreateParent(parentPath);

                var current = CreateNode<DirectoryNode>(directories[idx].Name, parent);
                directories[idx] = CreateWithPath(current.FullPath);
            }
            return directories;
        }

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

        private TPathNode CreateNode<TPathNode>(string nodeName, DirectoryNode parent) where TPathNode : APathNode, new()
        {
            if (parent is null) { throw new ArgumentNullException(nameof(parent)); }

            var pathNode = CreateNode<TPathNode>(nodeName);
            pathNode.SetParent(parent);
            return pathNode;
        }

        private TPathNode CreateNode<TPathNode>(string nodeName) where TPathNode : APathNode, new()
        {
            if (string.IsNullOrWhiteSpace(nodeName) || nodeName.Contains(Separator))
            {
                throw new ArgumentException($"expected name, but get empty name or path: {nodeName}");
            }
            return new TPathNode { Name = nodeName };
        }
    }

}
