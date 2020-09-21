using System.Windows;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    /// <summary>
    /// Interaction logic for RevitFamilyFilterOptionView.xaml
    /// </summary>
    public partial class RevitFamilyFilterView : Window
    {
        internal RevitFamilyFilterViewModel ViewModel { get { return DataContext as RevitFamilyFilterViewModel; } }

        public RevitFamilyFilterView()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
