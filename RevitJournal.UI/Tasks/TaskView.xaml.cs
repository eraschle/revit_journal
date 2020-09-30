using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

namespace RevitJournalUI.Tasks
{
    /// <summary>
    /// Interaction logic for JournalTaskListBoxItem.xaml
    /// </summary>
    public partial class TaskView : UserControl
    {
        private TaskViewModel ViewModel
        {
            get { return DataContext as TaskViewModel; }
        }

        public TaskView()
        {
            InitializeComponent();
        }


        private void OnTaskStatusUpdated(object sender, DataTransferEventArgs args)
        {
            if (!(sender is Button button) || ViewModel is null) { return; }

            var manager = ViewModel.TaskUoW.ReportManager;

            button.Visibility = manager.HasErrorAction ? Visibility.Visible : Visibility.Collapsed;
            if (manager.HasErrorAction)
            {
                button.Content = manager.ErrorAction.Name;
            }
            args.Handled = true;
        }
    }
}
