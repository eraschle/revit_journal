using DataSource.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Utilities.System;

namespace DataSource.Models.FileSystem
{
    public class FileSearch<TFile> where TFile : AFileNode, new()
    {
        public string Extension
        {
            get { return new TFile().FileExtension; }
        }

        public string Name { get; set; }

        public bool StartsWith { get; set; } = false;

        public bool EndsWith { get; set; } = false;

        public bool IsFile(AFileNode fileNode)
        {
            if (fileNode is null) { throw new ArgumentNullException(nameof(fileNode)); }

            if (fileNode.FileExtension != Extension)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(Name))
            {
                return true;
            }

            if (StartsWith && EndsWith == false)
            {
                var fileName = fileNode.NameWithoutExtension;
                return StringUtils.Starts(fileName, Name);
            }
            if (EndsWith && StartsWith == false)
            {
                var fileName = fileNode.NameWithoutExtension;
                return StringUtils.Ends(fileName, Name);
            }
            else
            {
                var fileName = fileNode.NameWithoutExtension;
                return StringUtils.Equals(fileName, Name);
            }
        }

    }
}
