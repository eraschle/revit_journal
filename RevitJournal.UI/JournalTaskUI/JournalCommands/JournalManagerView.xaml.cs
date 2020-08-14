using RevitJournal.Journal;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    /// <summary>
    /// Interaction logic for JournalCommandManagerView.xaml
    /// </summary>
    public partial class JournalManagerView : Window
    {
        public JournalManagerViewModel ViewModel { get { return DataContext as JournalManagerViewModel; } }

        public JournalManagerView(TaskManager manager)
        {
            InitializeComponent();
            ViewModel.UpdateCommands(manager);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
