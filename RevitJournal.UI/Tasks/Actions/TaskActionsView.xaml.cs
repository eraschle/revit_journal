using RevitJournal.Tasks;
using System.Windows;

namespace RevitJournalUI.Tasks.Actions
{
    /// <summary>
    /// Interaction logic for JournalCommandManagerView.xaml
    /// </summary>
    public partial class TaskActionsView : Window
    {
        public TaskActionsViewModel ViewModel { get { return DataContext as TaskActionsViewModel; } }

        public TaskActionsView(TaskManager manager)
        {
            InitializeComponent();
            ViewModel.UpdateAction(manager);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
