using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSource.Models.FileSystem
{
    public class DirectoryRootNode : DirectoryNode
    {
        private readonly string RootFullPath;
        private readonly IPathBuilder pathBuilder;

        public DirectoryRootNode(string path, IPathBuilder builder)
        {
            pathBuilder = builder ?? throw new ArgumentNullException(nameof(builder)); 
            Name = builder.GetLastNodeName(path);
            RootFullPath = builder.GetParentPath(path);
        }

        public override string FullPath
        {
            get { return Path.Combine(RootFullPath, Name); }
        }

        public TFile CreateFile<TFile>(string filePath) where TFile : AFileNode, new()
        {
            return pathBuilder.Create<TFile>(filePath);
        }

        public IList<string> GetFilePaths<TFile>(string pattern = null) where TFile : AFileNode, new()
        {
            var search = new TFile().GetSearchPattern(pattern);
            var option = SearchOption.AllDirectories;
            return Directory.GetFiles(FullPath, search, option).ToList();
        }

        public override string ToString()
        {
            return $"{base.ToString()} RootPath: {FullPath}";
        }
    }
}
