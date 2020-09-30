using DataSource.Model.FileSystem;
using System;
using System.Linq;
using Utilities.System;
using Utilities.System.FileSystem;

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

        private readonly IPathBuilder builder;

        public PathCreator(IPathBuilder pathBuilder)
        {
            builder = pathBuilder ?? throw new ArgumentNullException(nameof(pathBuilder));
        }

        public string RootPath { get; private set; }

        public string FileSuffix { get; set; } = string.Empty;

        public string NewRootPath { get; private set; } = string.Empty;

        public string BackupFolder { get; set; } = string.Empty;

        public bool AddBackupAtEnd { get; set; } = true;

        public void SetRoot(string path)
        {
            RootPath = string.IsNullOrEmpty(path) ? string.Empty : DirUtils.RemoveSlases(path);
        }

        public void SetNewRoot(string path)
        {
            NewRootPath = string.IsNullOrEmpty(path) ? string.Empty : DirUtils.RemoveSlases(path);
        }

        public TFile CreatePath<TFile>(TFile file) where TFile : AFileNode, new()
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            if (HasNewRootFolder())
            {
                file = builder.ChangeRoot(file, NewRootPath);
            }
            if (string.IsNullOrWhiteSpace(BackupFolder) == false)
            {
                file = AddBackupAtEnd 
                    ? builder.AddFolder(file, BackupFolder) 
                    : builder.InsertFolder(file, 1, BackupFolder);
            }
            if(file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if(file.HasParent(out var parent) == false)
            {
                throw new ArgumentException($"file {file.Name} has no parent");
            }
            var newFile = builder.Create<TFile>(file.Name, parent);
            if (parent.Exists() == false)
            {
                parent.Create();
            }

            if (string.IsNullOrWhiteSpace(FileSuffix) == false)
            {
                newFile.AddSuffixes(FileSuffix);
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
