using System.Windows;

namespace RevitJournalUI.Pages.Files.Filter
{
    /// <summary>
    /// Interaction logic for RevitFamilyFilterOptionView.xaml
    /// </summary>
    public partial class FileFilterView : Window
    {
        internal FileFilterViewModel ViewModel { get { return DataContext as FileFilterViewModel; } }

        public FileFilterView()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
