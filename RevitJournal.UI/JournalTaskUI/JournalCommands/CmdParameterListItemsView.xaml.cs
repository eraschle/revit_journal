using RevitJournal.Revit.SharedParameters;
using RevitJournalUI.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RevitJournalUI.JournalTaskUI.JournalCommands
{
    /// <summary>
    /// Interaction logic for CommandParameterListItemsView.xaml
    /// </summary>
    public partial class CmdParameterListItemsView : Window
    {
        public CmdParameterListItemsViewModel ViewModel { get { return DataContext as CmdParameterListItemsViewModel; } }
        public CmdParameterListItemsView(
            IList<SharedParameter> sharedParameters, 
            IList<string> preSelected)
        {
            InitializeComponent();
            ViewModel.UpdateParameterValues(sharedParameters, preSelected);
        }

        public ICollection<string> SelectedParameterNames
        {
            get { return ViewModel.CheckedSharedParameters()
                    .Select(par => par.Name).ToList(); }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs args)
        {
            if (!(lstItems.ItemsSource is ICollectionView collectionView)) { return; }

            collectionView.Refresh();
        }

        private void ItemFilter(object sender, FilterEventArgs args)
        {
            if (!(args.Item is CheckedDisplayViewModel model)) { return; }

            var lowerDisplay = model.DisplayName.ToLower(CultureInfo.CurrentCulture);
            var lowerSearch = txtSearch.Text.ToLower(CultureInfo.CurrentCulture);
            if (string.IsNullOrWhiteSpace(lowerDisplay)
                || string.IsNullOrWhiteSpace(lowerSearch))
            {
                args.Accepted = true;
            }
            else
            {
                args.Accepted = lowerDisplay.Contains(lowerSearch);
            }
        }
    }
}
