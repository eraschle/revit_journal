using DataSource.Helper;
using DataSource.Model.FileSystem;
using System;
using System.Drawing;
using System.IO;
using Utilities;

namespace RevitJournal.Helper
{
    public class PathCreator
    {

        public const string LibraryRootName = "[Library Folder]";
        public const string NewLibraryRootName = "[New Library Folder]";
        public const string LibraryPathName = "[Library Path]";
        public const string BackupFolderName = "[Save As Folder]";
        public const string RevitFileName = "[File Name]";
        public const string SuffixName = "[Save As Suffix]";

        private string RootPath { get; set; }

        private RevitFamilyFile RevitFile { get; set; }

        private string LibraryPath { get; set; }

        public string FileSuffix { get; set; } = string.Empty;

        public string NewRootPath { get; set; } = string.Empty;

        public string BackupFolder { get; set; } = string.Empty;

        public bool AddBackupAtEnd { get; set; } = true;

        public void SetFile(RevitFamilyFile revitFile)
        {
            if (revitFile is null)
            {
                throw new ArgumentNullException(nameof(revitFile));
            }
            RevitFile = revitFile;
            RootPath = RemoveSlases(revitFile.LibraryRoot);
            LibraryPath = RemoveSlases(revitFile.LibraryPath);
        }

        private string RemoveSlases(string path)
        {
            if (path.StartsWith(Constant.BackSlash, StringComparison.CurrentCulture))
            {
                path = path.Substring(0, 1);
            }

            if (path.EndsWith(Constant.BackSlash, StringComparison.CurrentCulture))
            {
                path = path.Substring(path.Length - 1);
            }
            return path;
        }

        public string CreatePath()
        {
            var rootPath = RootPath;
            if (HasNewRootFolder())
            {
                rootPath = NewRootPath;
            }
            if (string.IsNullOrWhiteSpace(BackupFolder))
            {
                rootPath = Path.Combine(rootPath, LibraryPath);
            }
            else
            {
                if (AddBackupAtEnd)
                {
                    rootPath = Path.Combine(rootPath, LibraryPath, BackupFolder);
                }
                else
                {
                    rootPath = Path.Combine(rootPath, BackupFolder, LibraryPath);
                }
            }
            if (Directory.Exists(rootPath) == false)
            {
                Directory.CreateDirectory(rootPath);
            }

            var fileName = string.Concat(RevitFile.Name, Constant.Point, RevitFile.Extension);
            if (string.IsNullOrWhiteSpace(FileSuffix) == false)
            {
                fileName = string.Concat(RevitFile.Name, Constant.Underline, FileSuffix, Constant.Point, RevitFile.Extension);
            }
            rootPath = Path.Combine(rootPath, fileName);
            return rootPath;
        }

        public string CreateSymbolic()
        {
            var symbolicPath = LibraryRootName;
            if (HasNewRootFolder())
            {
                symbolicPath = NewLibraryRootName;
            }
            if (string.IsNullOrWhiteSpace(BackupFolder))
            {
                symbolicPath = string.Join(Constant.BackSlash, symbolicPath, LibraryPathName);
            }
            else
            {
                if (AddBackupAtEnd)
                {
                    symbolicPath = string.Join(Constant.BackSlash, symbolicPath, LibraryPathName, BackupFolderName);
                }
                else
                {
                    symbolicPath = string.Join(Constant.BackSlash, symbolicPath, BackupFolderName, LibraryPathName);
                }
            }

            var fileName = RevitFileName;
            if (string.IsNullOrWhiteSpace(FileSuffix) == false)
            {
                fileName = string.Join(Constant.Underline, RevitFileName, SuffixName);
            }
            symbolicPath = string.Join(Constant.BackSlash, symbolicPath, fileName);
            return symbolicPath;
        }

        private bool HasNewRootFolder()
        {
            return string.IsNullOrWhiteSpace(NewRootPath) == false
                && StringUtils.Equals(RootPath, NewRootPath) == false;
        }
    }
}
