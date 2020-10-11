using DataSource.Models.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.System;
using Utilities.System.FileSystem;

namespace RevitJournal.Helper
{
    public class PathCreator
    {
        public const string RootName = "[Root Path]";
        public const string NewRootName = "[New ROOT Path]";
        public const string LibraryPathName = "[Library Path]";
        public const string NewFolderName = "[New Folder]";
        public const string FileName = "[File Name]";
        public const string FileSuffixName = "[File Suffix]";
        public const string NoPathConfigured = "Original path";

        private readonly IPathBuilder builder;

        public PathCreator(IPathBuilder pathBuilder)
        {
            builder = pathBuilder ?? throw new ArgumentNullException(nameof(pathBuilder));
        }

        public string SymbolicPath
        {
            get
            {
                return PathConfigured() ? CreateSymbolic() : NoPathConfigured;
            }
        }

        private string rootPath = string.Empty;
        public string RootPath
        {
            get { return rootPath; }
            set
            {
                rootPath = CheckPath(value);
                newRootPath = rootPath;
            }
        }

        public string FileSuffix { get; set; } = string.Empty;

        private string newRootPath = string.Empty;
        public string NewRootPath
        {
            get { return HasNewRootFolder() ? newRootPath : string.Empty; }
            set { newRootPath = DirUtils.RemoveSlases(value); }
        }

        public string NewFolder { get; set; } = string.Empty;

        public bool AddBackupAtEnd { get; set; } = true;

        private static string CheckPath(string path)
        {
            if (string.IsNullOrEmpty(path) || Directory.Exists(path) == false)
            {
                throw new ArgumentException($"Root path must exists {path}");
            }
            return DirUtils.RemoveSlases(path);
        }

        public TFile CreatePath<TFile>(TFile file) where TFile : AFileNode, new()
        {
            if (file is null) { throw new ArgumentNullException(nameof(file)); }

            if (HasNewRootFolder())
            {
                file = builder.ChangeRoot(file, NewRootPath);
            }
            if (string.IsNullOrWhiteSpace(NewFolder) == false)
            {
                file = AddBackupAtEnd
                    ? builder.AddFolder(file, NewFolder)
                    : builder.InsertFolder(file, 1, NewFolder);
            }
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            if (file.HasParent(out var parent) == false)
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
            var path = new List<string> { RootName };
            if (HasNewRootFolder())
            {
                path.Clear();
                path.Add(NewRootName);
            }
            path.Add(LibraryPathName);
            if (string.IsNullOrWhiteSpace(NewFolder) == false)
            {
                if (AddBackupAtEnd)
                {
                    path.Add(NewFolderName);
                }
                else
                {
                    path.Insert(1, NewFolderName);
                }
            }
            path.Add(FileName);
            if (string.IsNullOrWhiteSpace(FileSuffix) == false)
            {
                path.Add(FileSuffixName);
            }
            return string.Join(Constant.BackSlash, path.ToArray());
        }

        public bool PathConfigured()
        {
            return HasNewRootFolder()
                || string.IsNullOrWhiteSpace(NewFolder) == false
                || string.IsNullOrWhiteSpace(FileSuffix) == false;
        }

        private bool HasNewRootFolder()
        {
            return string.IsNullOrWhiteSpace(newRootPath) == false
                && StringUtils.Equals(rootPath, newRootPath) == false;
        }
    }
}
