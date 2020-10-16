using RevitJournalUI.Pages;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace RevitJournalUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel ViewModel
        {
            get { return DataContext as MainWindowModel; }
        }

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs args)
        {
            var content = args.Content;
            if(content is null || !(content is IPageView view)) { return; }

            view.SetModelData(args.ExtraData);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs args)
        {
            var uri = ViewModel.GetNext(out var model);
            MainFrame.Navigate(uri, model);
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var uri = ViewModel.GetPrevious(out var model);
            MainFrame.Navigate(uri, model);
        }
    }
}
