using System.Windows.Controls;
using System.Windows.Data;
using Utilities.System;

namespace RevitJournalUI.JournalTaskUI
{
    /// <summary>
    /// Interaction logic for JournalTaskOverviewView.xaml
    /// </summary>
    public partial class JournalTaskOverviewView : UserControl
    {
        private const string PrefixExecuted = "Executed Tasks";

        private TaskOverviewViewModel ViewModel
        {
            get { return DataContext as TaskOverviewViewModel; }
        }

        public JournalTaskOverviewView()
        {
            InitializeComponent();
        }

        private void ExecutedTasks_TargetUpdated(object sender, DataTransferEventArgs args)
        {
            var text = string.Join(Constant.Space, PrefixExecuted, ViewModel.ExecutedTasks, Constant.SlashChar, ViewModel.MaxTasks);
            lblExecutedTasks.Content = text;
        }
    }
}
