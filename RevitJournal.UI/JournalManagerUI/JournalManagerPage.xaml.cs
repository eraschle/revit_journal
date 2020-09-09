using RevitJournalUI.JournalTaskUI;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RevitJournalUI.JournalManagerUI
{
    /// <summary>
    /// Interaction logic for JournalManagerPage.xaml
    /// </summary>
    public partial class JournalManagerPage : Page
    {
        public JournalManagerPageModel ViewModel { get { return DataContext as JournalManagerPageModel; } }

        public JournalManagerPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await Task.Delay(2000).ConfigureAwait(true);
            Setup.Visibility = System.Windows.Visibility.Collapsed;
            Progess.Visibility = System.Windows.Visibility.Visible;
            using (var worker = MetadataBackgroundWorker.CreateWorker())
            {
                worker.ProgressChanged += new ProgressChangedEventHandler(OnProgressChanged);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnCompleted);
                worker.RunWorkerAsync(ViewModel.FamiliesViewModel.DirectoryViewModels);
            }
        }

        private void OnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Setup.Visibility = System.Windows.Visibility.Visible;
            Progess.Visibility = System.Windows.Visibility.Collapsed;
            ViewModel.FileFilterVisibility = System.Windows.Visibility.Visible;
            ViewModel.UpdateDuplicateName();
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ViewModel.SetProgress(e.ProgressPercentage);
        }
    }
}
