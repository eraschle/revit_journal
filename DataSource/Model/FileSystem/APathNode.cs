using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities;

namespace DataSource.Model.FileSystem
{
    public abstract class APathNode
    {
        protected DirectoryNode Parent { get; set; } = null;

        public bool HasParent(out DirectoryNode parent)
        {
            parent = null;
            if (Parent is object && !(Parent is DirectoryPathNode))
            {
                parent = Parent;
            }
            return parent is object;
        }

        public bool HasParent()
        {
            return HasParent(out _);
        }

        public abstract string Name { get; protected set; }

        public string FullPath
        {
            get { return GetPath(); }
        }

        public bool IsSame(string filePath)
        {
            return string.IsNullOrEmpty(filePath) == false
                && StringUtils.Equals(filePath, GetPath());
        }

        public virtual void SetPath(string path, DirectoryNode parent, IPathBuilder builder)
        {
            SetNodeName(path, builder);
            SetParentNode(parent);
        }

        protected void SetParentNode(DirectoryNode parent)
        {
            if (parent is null) { throw new ArgumentNullException(nameof(parent)); }

            Parent = parent;
        }

        protected void SetNodeName(string path, IPathBuilder builder)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            Name = builder.GetLast(path);
        }

        protected string GetPath()
        {
            return HasParent() ? Path.Combine(Parent.GetPath(), Name) : Name;
        }

        public abstract bool Exists();

        public abstract void Create();

        public abstract void Delete();

        public IEnumerable<APathNode> GetPathNodesToRoot()
        {
            return GetPathToRoot(this);
        }

        public IList<DirectoryNode> GetDirectoriesToRoot()
        {
            return GetDirectorisToRoot(Parent);
        }

        public DirectoryNode GetRoot(APathNode node)
        {
            if (node is null) { throw new ArgumentNullException(nameof(node)); }

            if (node.HasParent(out var parent))
            {
                return GetRoot(parent);
            }
            return node as DirectoryNode;
        }

        public string GetPathToRoot()
        {
            var pathNodes = GetPathNodesToRoot();
            var nodeNames = pathNodes.Select(node => node.Name);
            return Path.Combine(nodeNames.ToArray());
        }

        private List<DirectoryNode> GetDirectorisToRoot(DirectoryNode node)
        {
            if (node is null) { throw new ArgumentNullException(nameof(node)); }

            if (node.HasParent(out var parent))
            {
                var nodes = GetDirectorisToRoot(parent);
                nodes.Add(node);
                return nodes;
            }
            return new List<DirectoryNode>();
        }

        private List<APathNode> GetPathToRoot(APathNode node)
        {
            if (node is null) { throw new ArgumentNullException(nameof(node)); }

            if (node.HasParent(out var parent))
            {
                var nodes = GetPathToRoot(parent);
                nodes.Add(node);
                return nodes;
            }
            return new List<APathNode>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
