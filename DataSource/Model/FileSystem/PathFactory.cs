using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly Dictionary<string, DirectoryNode> directories = new Dictionary<string, DirectoryNode>();
        private readonly Dictionary<string, DirectoryPathNode> rootDirectories = new Dictionary<string, DirectoryPathNode>();

        private PathFactory() { }

        public bool HasRoot(string path)
        {
            return rootDirectories.Keys.Any(key => StringUtils.Starts(path, key));
        }

        public DirectoryPathNode GetRoot(string path)
        {
            CreateRoot(path);
            return rootDirectories[path];
        }

        public void CreateRoot(string path)
        {
            if (rootDirectories.ContainsKey(path)) { return; }

            var rootNode = CreateNode<DirectoryPathNode>(path, null);
            rootDirectories.Add(path, rootNode);
        }

        public static string RemoveLast(string path, string removeName)
        {
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(nameof(path)); }
            if (string.IsNullOrWhiteSpace(removeName)) { throw new ArgumentNullException(nameof(removeName)); }

            return path.Remove(path.Length - (removeName.Length + 1));
        }

        public string GetLastRemoved(string path, string removeName)
        {
            return RemoveLast(path, removeName);
        }

        public static string GetLastName(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(nameof(path)); }

            var splitPath = path.Split(Path.DirectorySeparatorChar);
            if (splitPath.Length == 1)
            {
                throw new ArgumentException($"Not valid path: {path}");
            }
            return splitPath.Last();
        }

        public string GetLast(string path)
        {
            return GetLastName(path);
        }

        public DirectoryNode Create(string path)
        {
            var parent = CreateParent(path);
            return Create(path, parent);
        }

        public DirectoryNode Create(string path, DirectoryNode parent)
        {
            if (HasRoot(path) == false) { throw new ArgumentException($"No Root created {path}"); }

            if (directories.ContainsKey(path) == false)
            {
                var current = CreateNode<DirectoryNode>(path, parent);
                directories.Add(path, current);
            }
            return directories[path];
        }

        public IList<DirectoryNode> Update(IList<DirectoryNode> directories)
        {
            if (directories is null) { throw new ArgumentNullException(nameof(directories)); }

            for (int idx = 1; idx < directories.Count; idx++)
            {
                var current = directories[idx];
                var parent = directories[idx - 1];
                directories[idx] = Create(current.FullPath, parent);
            }
            return directories;
        }


        public TFile Create<TFile>(string path) where TFile : AFileNode, new()
        {
            var parent = CreateParent(path);
            return CreateNode<TFile>(path, parent);
        }

        public TFile Create<TFile>(string path, DirectoryNode parent) where TFile : AFileNode, new()
        {
            return CreateNode<TFile>(path, parent);
        }

        private DirectoryNode CreateParent(string path)
        {
            var parentName = GetLast(path);
            var parentPath = GetLastRemoved(path, parentName);
            return Create(parentPath);
        }

        private TPathNode CreateNode<TPathNode>(string path, DirectoryNode parent) where TPathNode : APathNode, new()
        {
            var current = new TPathNode();
            current.SetPath(path, parent, this);
            return current;
        }
    }

}
