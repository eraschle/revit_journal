using DataSource.Helper;
using DataSource.Model.FileSystem;
using System;
using System.IO;
using Utilities;

namespace RevitJournal.Helper
{
    public class PathCreator
    {
        public const string LIBRARYROOTNAME = "[Library Folder]";
        public const string NEWLIBRARYROOTNAME = "[New Library Folder]";
        public const string LIBRARYPATHNAME = "[Library Path]";
        public const string BACKUPFOLDERNAME = "[Save As Folder]";
        public const string REVITFILENAME = "[File Name]";
        public const string SUFFIXNAME = "[Save As Suffix]";

        public string RootPath { get; private set; }

        public string FileSuffix { get; set; } = string.Empty;

        public string NewRootPath { get; private set; } = string.Empty;

        public string BackupFolder { get; set; } = string.Empty;

        public bool AddBackupAtEnd { get; set; } = true;

        public void SetRoot(string path)
        {
            RootPath = RemoveSlases(path is null ? string.Empty : path);
        }

        public void SetNewRoot(string path)
        {
            NewRootPath = RemoveSlases(path is null ? string.Empty : path);
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

        public string CreatePath(AFile file)
        {
            var rootPath = RootPath;
            if (HasNewRootFolder())
            {
                rootPath = NewRootPath;
            }
            var library = GetLibraryPath(file);
            if (string.IsNullOrWhiteSpace(BackupFolder))
            {
                rootPath = Path.Combine(rootPath, library);
            }
            else
            {
                if (AddBackupAtEnd)
                {
                    rootPath = Path.Combine(rootPath, library, BackupFolder);
                }
                else
                {
                    rootPath = Path.Combine(rootPath, BackupFolder, library);
                }
            }
            if (Directory.Exists(rootPath) == false)
            {
                Directory.CreateDirectory(rootPath);
            }

            var fileName = string.Concat(file.Name, Constant.Point, file.Extension);
            if (string.IsNullOrWhiteSpace(FileSuffix) == false)
            {
                fileName = string.Concat(file.Name, Constant.Underline, FileSuffix, Constant.Point, file.Extension);
            }
            rootPath = Path.Combine(rootPath, fileName);
            return rootPath;
        }

        private string GetLibraryPath(AFile file)
        {
            var library = file.ParentFolder.Replace(RootPath, string.Empty);
            return RemoveSlases(library);
        }

        public string CreateSymbolic()
        {
            var symbolicPath = LIBRARYROOTNAME;
            if (HasNewRootFolder())
            {
                symbolicPath = NEWLIBRARYROOTNAME;
            }
            if (string.IsNullOrWhiteSpace(BackupFolder))
            {
                symbolicPath = string.Join(Constant.BackSlash, symbolicPath, LIBRARYPATHNAME);
            }
            else
            {
                if (AddBackupAtEnd)
                {
                    symbolicPath = string.Join(Constant.BackSlash, symbolicPath, LIBRARYPATHNAME, BACKUPFOLDERNAME);
                }
                else
                {
                    symbolicPath = string.Join(Constant.BackSlash, symbolicPath, BACKUPFOLDERNAME, LIBRARYPATHNAME);
                }
            }

            var fileName = REVITFILENAME;
            if (string.IsNullOrWhiteSpace(FileSuffix) == false)
            {
                fileName = string.Join(Constant.Underline, REVITFILENAME, SUFFIXNAME);
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
