using System.Windows.Controls;
using System.Windows.Data;

namespace RevitJournalUI.Pages.Settings
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private const string prefixTimeoutTitle = "Timeout";
        private const string prefixParallelProcess = "Parallel Processes";

        //private SettingsPageModel ViewModel
        //{
        //    get { return DataContext as SettingsPageModel; }
        //}
        
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void OnTimeoutUpdated(object sender, DataTransferEventArgs args)
        {
            //if (!(sender is Label label)) { return; }

            //var timeout = DateUtils.AsString(ViewModel.Options.Timeout, format: DateUtils.Minute);
            //var timeoutTitle = string.Concat(prefixTimeoutTitle, " [", timeout, " min]");
            //label.Content = timeoutTitle;
            //args.Handled = true;
        }

        private void OnParallelProcessUpdated(object sender, DataTransferEventArgs args)
        {
            //if (!(sender is Label label)) { return; }

            //label.Content = string.Concat(prefixParallelProcess, " [", ViewModel.ParallelProcess, "]");
            //args.Handled = true;
        }
    }
}
