using DataSource.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSource.Model.FileSystem
{
    public class RevitDirectory : ANode
    {
        public override bool Exist
        {
            get { return Directory.Exists(FullPath); }
        }

        public RevitDirectory Parent { get; private set; }

        public List<RevitDirectory> Subfolder { get; } = new List<RevitDirectory>();

        public List<RevitFamily> Files { get; } = new List<RevitFamily>();


        public RevitDirectory(RevitDirectory parent, string fullPath, string libraryPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath)) { throw new ArgumentNullException(nameof(fullPath)); }

            Parent = parent;
            FullPath = GetFolderPath(fullPath);
            Name = GetName(fullPath);
            Subfolder.AddRange(GetSubfolders(fullPath, libraryPath));
            Files.AddRange(GetRevitFamilies(libraryPath));
        }

        private string GetFolderPath(string fullPath)
        {
            if (fullPath.Last() == Path.DirectorySeparatorChar)
            {
                fullPath = fullPath.Remove(fullPath.Length - 1);
            }
            return fullPath;
        }

        private string GetName(string fullPath)
        {
            return fullPath.Split(Path.DirectorySeparatorChar).Last();
        }

        private IEnumerable<RevitDirectory> GetSubfolders(string fullPath, string libraryPath)
        {
            return Directory.GetDirectories(fullPath)
                            .Where(dir => HasFiles(dir))
                            .Select(dir => new RevitDirectory(this, dir, libraryPath));
        }

        private bool HasFiles(string fullPath)
        {
            return Directory.GetFiles(fullPath, GetSearchPattern(), SearchOption.AllDirectories).Length > 0;
        }

        public IEnumerable<RevitFamily> GetRevitFamilies(string libraryPath)
        {
            return Directory.GetFiles(FullPath, GetSearchPattern(), SearchOption.TopDirectoryOnly)
                            .Select(filePath => new RevitFamilyFile { FullPath = filePath })
                            .Where(famFile => famFile.IsBackup == false)
                            .Select(famFile => new RevitFamily(famFile, libraryPath));
        }

        private string GetSearchPattern()
        {
            return string.Concat(Constant.Star, Constant.Point, RevitFamilyFile.FileExtension);
        }
    }
}
