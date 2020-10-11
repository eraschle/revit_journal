using DataSource.Model.Metadata;
using System.Windows;

namespace RevitJournalUI.MetadataUI
{
    /// <summary>
    /// Interaction logic for RevitMetadataDialog.xaml
    /// </summary>
    public partial class MetadataDialogView : Window
    {
        private MetadataDialogViewModel ViewModel
        {
            get
            {
                if (DataContext is MetadataDialogViewModel model)
                {
                    return model;
                }
                return null;
            }
        }
        public MetadataDialogView(Family family)
        {
            InitializeComponent();
            ViewModel.UpdateFamily(family);
        }

        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
