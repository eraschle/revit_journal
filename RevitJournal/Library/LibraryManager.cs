using System;
using RevitJournal.Tasks.Options;
using DataSource.Model.FileSystem;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace RevitJournal.Library
{
    public class LibraryManager
    {
        public LibraryRoot Root { get; private set; }

        public void CreateRoot(TaskOptions options)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            var rootDirectory = PathFactory.Instance.GetRoot(options.RootDirectory);
            var start = DateTime.Now;
            Debug.WriteLine($"Start: {start}");
            Root = new LibraryRoot(rootDirectory);
            Debug.WriteLine($"End  : {DateTime.Now - start}");
        }

        public bool HasRoot(out LibraryRoot rootHandler)
        {
            rootHandler = Root;
            return rootHandler is object;
        }

        public IList<RevitFamily> CheckedValidFiles()
        {
            return Root.CheckedFiles()
                       .Where(file => file.HasValidMetadata)
                       .ToList();
        }

        public IList<RevitFamily> EditableFiles()
        {
            return Root.CheckedFiles()
                       .Where(file => file.HasFileMetadata)
                       .ToList();
        }

        public IList<RevitFamily> GetCheckedFiles()
        {
            return Root.CheckedFiles().ToList();
        }
    }
}
