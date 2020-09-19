using DataSource.Model.FileSystem;
using System;

namespace RevitJournal.Library
{
    public class SelectFileHandler
    {
        public RevitFamily File { get; private set; }

        public SelectFolderHandler Folder { get; private set; }

        public bool IsSelected { get; set; } = true;

        public SelectFileHandler(RevitFamily revitFamily, SelectFolderHandler folder)
        {
            File = revitFamily ?? throw new ArgumentNullException(nameof(revitFamily));
            Folder = folder ?? throw new ArgumentNullException(nameof(folder));
        }
    }
}
