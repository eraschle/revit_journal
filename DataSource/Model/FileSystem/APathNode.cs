using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.System;

namespace DataSource.Model.FileSystem
{
    public abstract class APathNode : IEquatable<APathNode>
    {
        protected DirectoryNode Parent { get; set; }

        public abstract void SetParent(DirectoryNode parent);

        public abstract void RemoveParent();

        public bool HasParent(out DirectoryNode parent)
        {
            parent = Parent;
            return parent is object;
        }

        public string Name
        {
            get { return GetNodeName(); }
            set { SetNodeName(value); }
        }

        protected abstract void SetNodeName(string name);

        protected abstract string GetNodeName();

        public virtual string FullPath
        {
            get { return HasParent(out var parent) ? Path.Combine(parent.FullPath, Name) : Name; }
        }

        public string RootPath
        {
            get
            {
                var rootIncluded = false;
                var parents = GetRootParentNodes(rootIncluded);
                var names = parents.Select(node => node.Name)
                                   .Append(Name);
                return Path.Combine(names.ToArray());
            }
        }

        public abstract bool Exists();

        public abstract void Create();

        public abstract void Delete();

        public IList<DirectoryNode> GetRootParentNodes(bool included)
        {
            if( HasParent(out var parent))
            {
                return GetParentNodes(parent, included);
            }
            else if(included && this is DirectoryRootNode rootNode)
            {
                return new List<DirectoryNode> { rootNode };
            }
            else
            {
                return new List<DirectoryNode>();
            }
                
        }

        private IList<DirectoryNode> GetParentNodes(DirectoryNode directory, bool included)
        {
            if (directory.HasParent(out var parent))
            {
                var nodes = GetParentNodes(parent, included);
                nodes.Add(directory);
                return nodes;
            }
            else if (directory is DirectoryRootNode rootNode)
            {
                var nodes = new List<DirectoryNode>();
                if (included)
                {
                    nodes.Add(rootNode);
                }
                return nodes;
            }
            else
            {
                var parentMsg = parent is object ? parent.ToString() : "No Parent";
                var message = new StringBuilder();
                message.AppendLine("Node has no parent and is not a root node");
                message.AppendLine($"Path  : {directory.FullPath}");
                message.AppendLine($"Parent: {parentMsg}");
                throw new ArgumentException(message.ToString());
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as APathNode);
        }

        public bool Equals(APathNode other)
        {
            return other != null &&
                   FullPath == other.FullPath;
        }

        public override int GetHashCode()
        {
            return 2018552787 + EqualityComparer<string>.Default.GetHashCode(FullPath);
        }

        public static bool operator ==(APathNode left, APathNode right)
        {
            return EqualityComparer<APathNode>.Default.Equals(left, right);
        }

        public static bool operator !=(APathNode left, APathNode right)
        {
            return !(left == right);
        }
    }
}
