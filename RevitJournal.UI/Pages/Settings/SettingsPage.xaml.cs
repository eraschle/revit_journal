using System.Windows.Controls;

namespace RevitJournalUI.Pages.Settings
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page, IPageView
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        public APageModel ViewModel
        {
            get { return DataContext as APageModel; }
        }

        public void SetModelData(object data)
        {
            ViewModel.SetModelData(data);
        }
    }
}
