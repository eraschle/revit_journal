using RevitJournal.Revit.Filtering;
using RevitJournalUI.Pages.Files.Models;
using System.Windows.Controls;
using System.Windows.Data;

namespace RevitJournalUI.Pages.Files
{
    /// <summary>
    /// Interaction logic for FileSelectionPage.xaml
    /// </summary>
    public partial class TaskFilesPage : Page
    {
        public TaskFilesPage()
        {
            InitializeComponent();
        }

        private void DirectoryFilter(object sender, FilterEventArgs args)
        {
            if (!(args.Item is FolderModel model)) { return; }

            var folder = model.PathNode;
            args.Accepted &= RevitFilterManager.Instance.DirectoryFilter(folder);
        }
    }
}
