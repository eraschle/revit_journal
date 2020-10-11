using System;
using System.Windows;

namespace RevitJournalUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Uri("Pages/Files/TaskFilesPage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
