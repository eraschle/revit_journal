using System.Windows.Controls;
using System.Windows.Data;

namespace RevitJournalUI.JournalTaskUI
{
    /// <summary>
    /// Interaction logic for JournalTaskOverviewView.xaml
    /// </summary>
    public partial class JournalTaskOverviewView : UserControl
    {
        private TaskOverviewViewModel ViewModel
        {
            get { return DataContext as TaskOverviewViewModel; }
        }

        public JournalTaskOverviewView()
        {
            InitializeComponent();
        }

        private void ExecutedTasks_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            prgExecutedTasks.Value = ViewModel.ExecutedTasks;
        }
    }
}
