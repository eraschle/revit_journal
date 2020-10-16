using RevitJournalUI.Pages;
using RevitJournalUI.Pages.Files;
using RevitJournalUI.Pages.Settings;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Utilities.UI;

namespace RevitJournalUI
{
    public class MainWindowModel : ANotifyPropertyChangedModel
    {
        private readonly LinkedList<Uri> pages = new LinkedList<Uri>();
        private readonly Dictionary<Uri, APageModel> pagesMap = new Dictionary<Uri, APageModel>();

        public MainWindowModel()
        {
            AddPage("Pages/Settings/SettingsPage.xaml", new SettingsPageModel());
            AddPage("Pages/Files/TaskFilesPage.xaml", new TaskFilesPageModel());
        }

        private void AddPage(string uri, APageModel pageModel)
        {
            var uriPage = new Uri(uri, UriKind.RelativeOrAbsolute);
            pages.AddLast(uriPage);
            pagesMap.Add(uriPage, pageModel);
        }

        private LinkedListNode<Uri> CurrentPage { get; set; }

        public Uri GetNext(out APageModel model)
        {
            if (CurrentPage is null)
            {
                model = null;
                CurrentPage = pages.First;
            }
            else
            {
                model = pagesMap[CurrentPage.Value];
                CurrentPage = CurrentPage.Next;
            }
            return CurrentPage.Value;
        }

        internal Uri GetPrevious(out APageModel model)
        {
            if (CurrentPage.Previous is null)
            {
                model = null;
                return null;
            }

            model = pagesMap[CurrentPage.Value];
            CurrentPage = CurrentPage.Previous;
            return CurrentPage.Value;
        }

        public StatusBarViewModel StatusBar
        {
            get { return StatusBarViewModel.Instance; }
        }
    }
}
