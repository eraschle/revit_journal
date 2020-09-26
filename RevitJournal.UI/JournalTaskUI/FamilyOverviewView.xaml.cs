using RevitJournal.Revit.Filtering;
using RevitJournalUI.JournalTaskUI.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RevitJournalUI.JournalTaskUI
{
    public partial class FamilyOverviewView : UserControl
    {
        private FamilyOverviewViewModel ViewModel { get { return DataContext as FamilyOverviewViewModel; } }

        public FamilyOverviewView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            if (args.NewValue is DirectoryViewModel newModel)
            {
                ViewModel.SelectedDirectory = newModel;
                ViewModel.UpdateFamilyViewModels(newModel);
                args.Handled = true;
            }
        }

        private void DirectoryFilter(object sender, FilterEventArgs args)
        {
            if (!(args.Item is DirectoryViewModel model)) { return; }

            var folder = model.Handler.Folder;
            args.Accepted &= RevitFilterManager.Instance.DirectoryFilter(folder);
        }

        private void FileFilter(object sender, FilterEventArgs args)
        {
            if (!(args.Item is FamilyViewModel model)) { return; }

            var file = model.Handler.File;
            args.Accepted &= RevitFilterManager.Instance.FileFilter(file);
        }

        private void Combobox_Filter_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (trvDirectories.ItemsSource is ICollectionView view)
            {
                view.Refresh();
                args.Handled = true;
            }
        }
    }
}
