using System;
using RevitJournal.Tasks.Options;
using DataSource.Models.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace RevitJournal.Library
{
    public class LibraryManager
    {
        public LibraryRoot Root { get; private set; }

        public IPathBuilder PathBuilder { get; set; }

        public void CreateRoot(TaskOptions options)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            var rootDirectory = PathBuilder.CreateRoot(options.RootDirectory.Value);
            //PathBuilder.CreateFiles<RevitFamilyFile>(rootDirectory);
            Root = new LibraryRoot(rootDirectory);
        }

        public bool HasRoot(out LibraryRoot rootHandler)
        {
            rootHandler = Root;
            return rootHandler is object;
        }

        public IList<RevitFamilyFile> CheckedValidFiles()
        {
            return Root.CheckedFiles()
                       .Where(file => file.AreMetadataValid)
                       .ToList();
        }

        public IList<RevitFamilyFile> EditableFiles()
        {
            return Root.CheckedFiles()
                       .Where(file => file.HasFileMetadata)
                       .ToList();
        }

        public IList<RevitFamilyFile> GetCheckedFiles()
        {
            return Root.CheckedFiles().ToList();
        }
    }
}
