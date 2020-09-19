using System;
using RevitJournal.Tasks.Options;
using DataSource.Model.FileSystem;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;

namespace RevitJournal.Library
{
    public class LibraryManager
    {
        public FilterManager FilterManager { get; set; } = new FilterManager();

        public SelectFolderHandler Root { get; private set; }

        public SelectFolderHandler CreateRoot(TaskOptions options)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            var rootDirectory = new RevitDirectory(null, options.RootDirectory, options.RootDirectory);
            Root = new SelectFolderHandler(rootDirectory);
            return Root;
        }

        public IList<RevitFamily> GetCheckedValidFiles()
        {
            return Root.SelectedRecusiveFiles(FilterManager)
                       .Where(handler => handler.File.HasValidMetadata)
                       .Select(handler => handler.File)
                       .ToList();
        }

        public IList<RevitFamily> GetEditableRecursiveFiles()
        {
            return Root.SelectedRecusiveFiles(FilterManager)
                       .Where(handler => handler.File.HasFileMetadata)
                       .Select(handler => handler.File)
                       .ToList();
        }

        public IList<RevitFamily> GetCheckedFiles()
        {
            return Root.SelectedRecusiveFiles(FilterManager)
                       .Select(handler => handler.File)
                       .ToList();
        }
    }
}
