using DataSource.Models.FileSystem;
using RevitJournal.Revit.Filtering;
using RevitJournal.Tasks.Options;
using RevitJournalUI.Pages.Files.Models;
using RevitJournalUI.Pages.Files.Worker;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace RevitJournalUI.Pages.Files
{
    /// <summary>
    /// Interaction logic for FileSelectionPage.xaml
    /// </summary>
    public partial class TaskFilesPage : Page, IPageView
    {
        public TaskFilesPage()
        {
            InitializeComponent();
        }

        private void DirectoryFilter(object sender, FilterEventArgs args)
        {
            if (!(args.Item is FolderModel model)) { return; }

            var folder = model.PathNode as DirectoryNode;
            args.Accepted &= RevitFilterManager.Instance.DirectoryFilter(folder);
        }

        public APageModel ViewModel
        {
            get { return DataContext as APageModel; }
        }

        public void SetModelData(object data)
        {
            ViewModel.SetModelData(data);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var worker = MetadataWorker.Create())
            {
                worker.RunWorkerAsync(TaskOptions.Instance);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            if (args.Handled || !(ViewModel is TaskFilesPageModel pageModel)
                || !(args.NewValue is PathModel newModel)) { return; }

            if (newModel.Parent is object)
            {
                foreach (var child in newModel.Parent.ChildFolders)
                {
                    child.IsExpanded = child.Equals(newModel);
                }
            }
            else if (newModel is FolderModel rootModel)
            {
                foreach (var child in rootModel.ChildFolders)
                {
                    child.IsExpanded = false;
                }
            }
            pageModel.SetSelectedModel(newModel);
            args.Handled = true;
        }

        private void Expander_Checked(object sender, RoutedEventArgs e)
        {
            if(!(sender is ToggleButton button)
                || !(button.DataContext is PathModel pathModel)) { return; }

            pathModel.IsExpanded = button.IsChecked == true;
        }
    }
}
