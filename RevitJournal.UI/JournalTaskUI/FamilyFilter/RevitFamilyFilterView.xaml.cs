using RevitJournalUI.JournalTaskUI.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    /// <summary>
    /// Interaction logic for RevitFamilyFilterOptionView.xaml
    /// </summary>
    public partial class RevitFamilyFilterView : Window
    {
        internal RevitFamilyFilterViewModel ViewModel { get { return DataContext as RevitFamilyFilterViewModel; } }

        public RevitFamilyFilterView(ObservableCollection<DirectoryViewModel> viewModels, FilterManager filterManager)
        {
            InitializeComponent();
            ViewModel.LoadFamilyMetadata(viewModels, filterManager);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
