using DataSource.Comparer;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using RevitJournalUI.Helper;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace RevitJournalUI.MetadataUI
{
    /// <summary>
    /// Interaction logic for RevitMetadataDialog.xaml
    /// </summary>
    public partial class MetadataEditDialogView : Window
    {
        private MetadataEditDialogViewModel ViewModel { get { return DataContext as MetadataEditDialogViewModel; } }
        private readonly FamilyEditedComparer EditedComparer = new FamilyEditedComparer();

        public MetadataEditDialogView(IList<RevitFamily> families)
        {
            InitializeComponent();
            ViewModel.Update(families);
        }

        private void BtnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            foreach (var family in ViewModel.Families)
            {
                var revitFamily = ViewModel.GetRevitFamily(family);
                revitFamily.Update();
                var reloaded = revitFamily.Metadata;
                if (revitFamily != null && EditedComparer.Equals(reloaded, family) == false)
                {
                    revitFamily.SetExternalEditDataSource();
                    revitFamily.Write(family);
                }
            }
            DialogResult = true;
        }

        private void LstFamilies_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args is null || args.AddedItems.Count == 0 || !(args.AddedItems[0] is Family family)) { return; }

            ViewModel.Update(family);
            ListViewHelper.SizeColumns(lstFamilyParameters);
            ListViewHelper.SizeColumns(lstFamilyTypeParameters);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListViewHelper.SizeColumns(lstFamilyParameters);
            ListViewHelper.SizeColumns(lstFamilyTypeParameters);
        }
    }
}
