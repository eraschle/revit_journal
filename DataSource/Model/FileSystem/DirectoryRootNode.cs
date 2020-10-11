using System;
using System.IO;

namespace DataSource.Models.FileSystem
{
    public class DirectoryRootNode : DirectoryNode
    {
        private readonly string RootFullPath;

        public DirectoryRootNode(string path, IPathBuilder builder)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            Name = builder.GetLastNodeName(path);
            RootFullPath = builder.GetParentPath(path);
        }

        public override string FullPath
        {
            get { return Path.Combine(RootFullPath, Name); }
        }

        public override string ToString()
        {
            return $"{base.ToString()} RootPath: {FullPath}";
        }
    }
}
