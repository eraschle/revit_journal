using DataSource.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSource.Model.FileSystem
{
    public class RevitDirectory : ANode
    {
        public override bool Exist { get { return Directory.Exists(FullPath); } }

        public bool IsRoot { get { return Parent is null; } }

        public RevitDirectory Parent { get; private set; }

        public RevitDirectory Root { get { return GetRootDirectory(this); } }

        private RevitDirectory GetRootDirectory(RevitDirectory directory)
        {
            if (directory.IsRoot) { return directory; }

            return GetRootDirectory(directory.Parent);
        }

        public IList<RevitDirectory> SubDirectories { get { return GetSubDirectories(); } }

        public RevitDirectory(RevitDirectory parent, string fullPath)
        {
            Parent = parent;
            var separator = Path.DirectorySeparatorChar.ToString();
            if (fullPath.EndsWith(separator, StringComparison.CurrentCulture))
            {
                fullPath = fullPath.Replace(separator, string.Empty);
            }
            FullPath = fullPath;
            Name = FullPath.Split(Path.DirectorySeparatorChar).Last();
        }

        private IList<RevitDirectory> GetSubDirectories()
        {
            var dirs = Directory.GetDirectories(FullPath)
                                .Select(dir => new RevitDirectory(this, dir))
                                .Where(dir => dir.HasFiles(true))
                                .ToList();
            return dirs;
        }

        private bool HasFiles(bool includeSubDirectories)
        {
            return GetFiles(RevitFamilyFile.FileExtension, includeSubDirectories).Count > 0;
        }

        public IList<RevitFamily> GetRevitFamilies(string libraryPath, bool includeSubDirectories)
        {
            return GetFiles(RevitFamilyFile.FileExtension, includeSubDirectories)
                .Select(file => new RevitFamily(file, libraryPath))
                .ToList();
        }

        private IList<RevitFamilyFile> GetFiles(string extention, bool recursive)
        {
            var search = string.Concat(string.Concat(Constant.Star, Constant.Point), extention);
            var options = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(FullPath, search, options)
                                 .Select(file => AFile.Create<RevitFamilyFile>(file))
                                 .Where(file => file.IsBackup == false)
                                 .ToList();
            return files;
        }
    }
}
