using System;
using System.IO;

namespace DataSource.Model.FileSystem
{
    public class DirectoryRootNode : DirectoryNode
    {
        private readonly string RootPath;

        public DirectoryRootNode(string path, IPathBuilder builder)
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            Name = builder.GetLastNodeName(path);
            RootPath = builder.GetParentPath(path);
        }

        public override string FullPath
        {
            get { return Path.Combine(RootPath, Name); }
        }

        public override string ToString()
        {
            return $"{base.ToString()} RootPath: {RootPath}";
        }
    }
}
