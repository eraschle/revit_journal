using RevitJournalUI.JournalTaskUI.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RevitJournalUI.JournalTaskUI
{
    /// <summary>
    /// Interaction logic for RevitFileOverviewView.xaml
    /// </summary>
    public partial class FamilyOverviewView : UserControl
    {
        private FamilyOverviewViewModel ViewModel { get { return DataContext as FamilyOverviewViewModel; } }

        public FamilyOverviewView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            if (args.OldValue is DirectoryViewModel oldModel)
            {
                oldModel.Selected = false;
            }
            if (args.NewValue is DirectoryViewModel newModel)
            {
                newModel.Selected = true;

                ViewModel.SelectedDirectory = newModel;
                ViewModel.UpdateFamilyViewModels(newModel);
                args.Handled = true;
            }
        }

        private void DirectoryFilter(object sender, FilterEventArgs args)
        {
            args.Accepted &= ViewModel.DirectoryFilter(args.Item);
        }

        private void FileFilter(object sender, FilterEventArgs args)
        {
            args.Accepted &= ViewModel.FileFilter(args.Item);
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
