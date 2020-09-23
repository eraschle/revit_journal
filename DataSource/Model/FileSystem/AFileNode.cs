using DataSource.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Utilities;

namespace DataSource.Model.FileSystem
{
    public abstract class AFileNode : APathNode
    {
        public override string Name
        {
            get { return GetFileName(); }
            protected set { SetFileName(value); }
        }

        private void SetFileName(string fileName)
        {
            if (StringUtils.Ends(fileName, FileExtension))
            {
                fileName = PathFactory.RemoveLast(fileName, FileExtension);
            }
            NameWithoutExtension = fileName;
        }

        private string GetFileName()
        {
            var fileName = NameWithoutExtension;
            if (NameSuffixes.Count > 0)
            {
                NameSuffixes.Insert(0, NameWithoutExtension);
                fileName = string.Join(Constant.Underline, NameSuffixes);
            }
            return string.Join(Constant.Point, fileName, FileExtension);
        }

        [JsonIgnore]
        public string NameWithoutExtension { get; private set; }

        public abstract string FileExtension { get; }

        public List<string> NameSuffixes { get; } = new List<string>();

        public override bool Exists()
        {
            return File.Exists(GetPath());
        }

        public override void Create()
        {
            if (Exists()) { return; }

            using (var stream = File.Create(GetPath()))
            {
                stream.Close();
            }
        }

        public override void Delete()
        {
            if (Exists() == false) { return; }

            File.Delete(GetPath());
        }

        public string GetSearchPattern(string pattern = null)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                pattern = Constant.Star;
            }
            return string.Concat(pattern, Constant.Point, FileExtension);
        }

        public TFile ChangeExtension<TFile>() where TFile : AFileNode, new()
        {
            return new TFile { Parent = Parent, NameWithoutExtension = NameWithoutExtension };
        }

        public TFile ChangeFileName<TFile>(string newFileName) where TFile : AFileNode, new()
        {
            return new TFile() { Parent = Parent, NameWithoutExtension = newFileName };
        }

        public TFile ChangeDirectory<TFile>(string newDirectory, IPathBuilder builder) where TFile : AFileNode, new()
        {
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            var parent = builder.Create(newDirectory);
            return ChangeDirectory<TFile>(parent);
        }

        public TFile ChangeDirectory<TFile>(DirectoryNode newDirectory) where TFile : AFileNode, new()
        {
            if (newDirectory is null) { throw new ArgumentNullException(nameof(newDirectory)); }

            return new TFile { Parent = newDirectory, NameWithoutExtension = NameWithoutExtension };
        }

        public TFile CopyTo<TFile>(TFile destination, bool overrideFile = false) where TFile : AFileNode, new()
        {
            if (destination is null) { throw new ArgumentNullException(nameof(destination)); }

            File.Copy(GetPath(), destination.FullPath, overrideFile);
            return destination;
        }

        public override bool Equals(object obj)
        {
            return obj is AFileNode file &&
                   FullPath == file.FullPath;
        }

        public override int GetHashCode()
        {
            return 2018552787 + EqualityComparer<string>.Default.GetHashCode(FullPath);
        }
    }
}
