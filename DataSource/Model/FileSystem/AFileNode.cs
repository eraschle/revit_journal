using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Utilities.System;

namespace DataSource.Models.FileSystem
{
    public abstract class AFileNode : APathNode
    {
        [JsonIgnore]
        public string NameWithoutExtension { get; private set; }

        protected override string GetNodeName()
        {
            var fileName = NameWithoutExtension;
            if (NameSuffixes.Count > 0)
            {
                var names = new List<string> { fileName };
                names.AddRange(NameSuffixes);
                fileName = string.Join(Constant.Underline, names);
            }
            return string.Join(Constant.Point, fileName, FileExtension);
        }

        protected override void SetNodeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) { return; }

            if (HasFileExtension(name))
            {
                name = RemoveFileExtension(name);
            }
            NameWithoutExtension = name;
        }

        protected bool HasFileExtension(string filename)
        {
            return string.IsNullOrEmpty(filename) == false
                && StringUtils.Ends(filename, $"{Constant.Point}{FileExtension}");
        }

        protected string RemoveFileExtension(string filename)
        {
            if (string.IsNullOrEmpty(filename) || HasFileExtension(filename) == false)
            {
                return filename;
            }
            var extLength = FileExtension.Length;
            var nameLength = filename.Length;
            if (filename.Length > extLength)
            {
                var pointIdx = extLength + 1;
                var point = filename[nameLength - pointIdx];
                if (point == Constant.PointChar)
                {
                    extLength += 1;
                }
            }
            var removeIdx = nameLength - extLength;
            return filename.Remove(removeIdx);
        }

        [JsonIgnore]
        public abstract string FileExtension { get; }

        private List<string> NameSuffixes { get; } = new List<string>();

        public void AddSuffixes(params string[] suffixes)
        {
            if (suffixes is null || suffixes.Length == 0) { return; }

            NameSuffixes.AddRange(suffixes);
        }

        public override bool Exists()
        {
            return File.Exists(FullPath);
        }

        public override void Create()
        {
            if (Exists()) { return; }

            File.WriteAllText(FullPath, string.Empty);
        }

        public override void Delete()
        {
            if (Exists() == false) { return; }

            File.Delete(FullPath);
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
            var changed = new TFile { NameWithoutExtension = NameWithoutExtension };
            changed.SetParent(Parent);
            return changed;
        }

        public TFile ChangeFileName<TFile>(string fileName) where TFile : AFileNode, new()
        {
            var changed = new TFile { Name = fileName };
            changed.SetParent(Parent);
            return changed;
        }

        public TFile ChangeDirectory<TFile>(DirectoryNode newDirectory) where TFile : AFileNode, new()
        {
            if (newDirectory is null) { throw new ArgumentNullException(nameof(newDirectory)); }

            var changed = new TFile { NameWithoutExtension = NameWithoutExtension };
            changed.SetParent(newDirectory);
            return changed;
        }

        public TFile CopyTo<TFile>(TFile destination, bool overrideFile = false) where TFile : AFileNode, new()
        {
            if (destination is null) { throw new ArgumentNullException(nameof(destination)); }

            File.Copy(FullPath, destination.FullPath, overrideFile);
            return destination;
        }
    }
}
