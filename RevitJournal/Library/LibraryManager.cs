using System;
using RevitJournal.Tasks.Options;
using DataSource.Model.FileSystem;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using RevitJournal.Revit.Filtering;
using RevitJournal.Library.Filtering;

namespace RevitJournal.Library
{
    public class LibraryManager
    {
        public static bool HasFilterManager(out RevitFilterManager manager)
        {
            manager = FilterManager;
            return manager is object;
        }

        public static RevitFilterManager FilterManager { get; } = new RevitFilterManager();

        public LibraryRoot Root { get; private set; }

        public void CreateRoot(TaskOptions options)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            var rootDirectory = new RevitDirectory(null, options.RootDirectory, options.RootDirectory);
            Root = new LibraryRoot(rootDirectory);
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
