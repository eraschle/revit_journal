using DataSource.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataSource.Model.FileSystem
{
    public abstract class AFile : ANode
    {
        public static TFile Create<TFile>(string path) where TFile : AFile, new()
        {
            var file = new TFile() { FullPath = path };
            return file;
        }

        private static string CreatePath(string dir, string fileName, string extension)
        {
            fileName = string.Concat(fileName, Constant.PointChar, extension);
            return Path.Combine(dir, fileName);
        }

        public override string FullPath
        {
            get { return Path.Combine(ParentFolder, NameWithExtension); }
            set { SetFilePath(value); }
        }

        [JsonIgnore]
        public string NameWithExtension { get { return string.Concat(Name, Constant.PointChar, Extension); } }

        [JsonIgnore]
        public string ParentFolder { get; protected set; } = string.Empty;

        [JsonIgnore]
        public string Extension { get; protected set; } = string.Empty;

        public override bool Exist { get { return File.Exists(FullPath); } }

        protected void SetFilePath(string filePath)
        {
            Name = Path.GetFileNameWithoutExtension(filePath);
            Extension = Path.GetExtension(filePath).Replace(Constant.Point, string.Empty);
            ParentFolder = Path.GetDirectoryName(filePath);
            CheckExtension();
        }

        private void CheckExtension()
        {
            var extension = GetExtension();
            if (Extension.Equals(extension, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                var message = $"{Extension} is not a {GetTypeName()} Extension [{extension}]";
                throw new ArgumentException(message);
            }
        }

        protected abstract string GetExtension();

        protected abstract string GetTypeName();

        public bool IsSame(string filePath)
        {
            return string.IsNullOrEmpty(filePath) == false
                && filePath.Equals(FullPath, StringComparison.CurrentCulture);
        }

        public TFile ChangeExtension<TFile>(string newExtension) where TFile : AFile, new()
        {
            var newFilePath = CreatePath(ParentFolder, Name, newExtension);
            return Create<TFile>(newFilePath);
        }

        public TFile ChangeFileName<TFile>(string newFileName) where TFile : AFile, new()
        {
            var newFilePath = CreatePath(ParentFolder, newFileName, Extension);
            return Create<TFile>(newFilePath);
        }

        public TFile ChangeDirectory<TFile>(string newDirectory) where TFile : AFile, new()
        {
            var newFilePath = CreatePath(newDirectory, Name, Extension);
            return Create<TFile>(newFilePath);
        }

        public TFile CopyTo<TFile>(string destinationPath, bool overrideFile = false) where TFile : AFile, new()
        {
            File.Copy(FullPath, destinationPath, overrideFile);
            return Create<TFile>(destinationPath);
        }

        public TFile CopyTo<TFile>(AFile destination, bool overrideFile = false) where TFile : AFile, new()
        {
            if (destination is null) { return null; }

            return CopyTo<TFile>(destination.FullPath, overrideFile);
        }

        public void Delete()
        {
            if (Exist)
            {
                File.Delete(FullPath);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is AFile file &&
                   FullPath == file.FullPath;
        }

        public override int GetHashCode()
        {
            return 2018552787 + EqualityComparer<string>.Default.GetHashCode(FullPath);
        }
    }
}
