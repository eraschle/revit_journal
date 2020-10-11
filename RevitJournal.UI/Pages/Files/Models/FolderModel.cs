using DataSource.Models.FileSystem;

namespace RevitJournalUI.Pages.Files.Models
{
    public class FolderModel : PathModel<DirectoryNode>
    {
        public FolderModel(DirectoryNode pathNode, FolderModel parent) : base(pathNode, parent)
        {
        }
    }
}
