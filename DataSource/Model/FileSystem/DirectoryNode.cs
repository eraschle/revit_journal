using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSource.Model.FileSystem
{
    public class DirectoryNode : APathNode
    {
        public override string Name { get; protected set; }

        public DirectoryNode() : base() { }

        public DirectoryNode(DirectoryNode directory) : base()
        {
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }

            Parent = directory.Parent;
            Name = directory.Name;
        }

        public override bool Exists()
        {
            return Directory.Exists(GetPath());
        }

        public override void Create()
        {
            if (Exists()) { return; }

            Directory.CreateDirectory(GetPath());
        }

        public override void Delete()
        {
            if (Exists() == false) { return; }

            Directory.Delete(GetPath());
        }

        public IEnumerable<DirectoryNode> GetSubfolders()
        {
            return Directory.GetDirectories(FullPath)
                            .Select(dir => PathFactory.Instance.Create(dir, this));
        }

        public IEnumerable<DirectoryNode> GetSubfolders<TFile>() where TFile : AFileNode, new()
        {
            return Directory.GetDirectories(FullPath)
                            .Where(dir => HasFiles<TFile>(true))
                            .Select(dir => PathFactory.Instance.Create(dir, this));
        }

        public bool HasFiles<TFile>(bool recursive) where TFile : AFileNode, new()
        {
            return HasFiles<TFile>(recursive, out _);
        }

        public bool HasFiles<TFile>(bool recursive, out IEnumerable<TFile> files) where TFile : AFileNode, new()
        {
            files = GetFiles<TFile>(recursive);
            return files.Any();
        }

        public IEnumerable<TFile> GetFiles<TFile>(bool recursive, string pattern = null) where TFile : AFileNode, new()
        {
            var option = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            return Directory.GetFiles(FullPath, new TFile().GetSearchPattern(pattern), option)
                            .Select(path => PathFactory.Instance.Create<TFile>(path, this));
        }
    }
}
