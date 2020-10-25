using DataSource.Models.FileSystem;
using RevitJournal.Revit.Filtering;
using RevitJournal.Tasks.Options;
using RevitJournalUI.Pages.Files.Models;
using RevitJournalUI.Pages.Files.Worker;
using System.Windows;
using System.Windows.Controls;
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
            if (!(ViewModel is TaskFilesPageModel pageModel)) { return; }

            pageModel.SetSelectedModel(args.NewValue as PathModel);
        }
    }
}
