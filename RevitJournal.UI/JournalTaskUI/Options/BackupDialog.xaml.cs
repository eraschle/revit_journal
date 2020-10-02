using RevitJournal.Helper;
using System;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.Options
{
    /// <summary>
    /// Interaction logic for DublicatedManagerPageView.xaml
    /// </summary>
    public partial class BackupDialog : Window
    {
        private BackupDialogModel ViewModel
        {
            get { return DataContext as BackupDialogModel; }
        }

        public BackupDialog()
        {
            InitializeComponent();
        }

        public void Update(PathCreator creator)
        {
            ViewModel.Update(creator);
        }

        public PathCreator GetPathCreator()
        {
            return ViewModel.GetPathCreator();
        }

        private void ButtonOkay_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
