using DataSource.Helper;
using DataSource.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                path = path.Substring(1);
            }

            if (path.EndsWith(Constant.BackSlash, StringComparison.CurrentCulture))
            {
                path = path.Substring(path.Length - 1);
            }
            return path;
        }

        public TFile CreatePath<TFile>(TFile file) where TFile : AFileNode, new()
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            var factory = PathFactory.Instance;
            var directories = file.GetDirectoriesToRoot();
            DirectoryNode rootNode = factory.GetRoot(RootPath);
            if (HasNewRootFolder())
            {
                directories.Remove(rootNode);
                var newRootNode = factory.GetRoot(NewRootPath);
                directories.Insert(0, newRootNode);
                factory.Update(directories);
            }
            if (string.IsNullOrWhiteSpace(BackupFolder) == false)
            {
                if (AddBackupAtEnd)
                {
                    var backupNode = factory.Create(BackupFolder, directories.Last());
                    directories.Add(backupNode);
                }
                else
                {
                    var backupNode = factory.Create(BackupFolder, directories.First());
                    directories.Insert(1, backupNode);
                    factory.Update(directories);
                }
            }
            var newFile = factory.Create<TFile>(file.FullPath, directories.Last());
            if (newFile.HasParent(out var parent) 
                && parent.Exists() == false)
            {
                parent.Create();
            }

            if (string.IsNullOrWhiteSpace(FileSuffix) == false)
            {
                newFile.NameSuffixes.Add(FileSuffix);
            }
            return newFile;
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
