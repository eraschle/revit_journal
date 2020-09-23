using System;

namespace DataSource.Model.FileSystem
{
    public class DirectoryPathNode : DirectoryNode
    {
        public override void SetPath(string path, DirectoryNode parent, IPathBuilder builder)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            SetNodeName(path, builder);
            var rootName = builder.GetLastRemoved(path, Name);
            SetParentNode(new DirectoryPathNode { Name = rootName });
        }
    }
}
