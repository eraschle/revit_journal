using RevitAction.Action;
using RevitJournal.Tasks;
using System.Collections.Generic;
using System.Windows;

namespace RevitJournalUI.Tasks.Actions
{
    /// <summary>
    /// Interaction logic for JournalCommandManagerView.xaml
    /// </summary>
    public partial class TaskActionsView : Window
    {
        public TaskActionsViewModel ViewModel { get { return DataContext as TaskActionsViewModel; } }

        public TaskActionsView(IEnumerable<ITaskAction> taskActions)
        {
            InitializeComponent();
            ViewModel.UpdateAction(taskActions);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
