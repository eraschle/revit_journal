using DataSource.Model.FileSystem;
using RevitJournal.Library;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Utilities;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class DirectoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<FamilyViewModel> RecursiveFamilies = new List<FamilyViewModel>();

        internal SelectFolderHandler FolderHandler { get; private set; }

        public DirectoryViewModel(SelectFolderHandler folderHandler)
        {
            if (folderHandler is null) { throw new ArgumentNullException(nameof(folderHandler)); }

            FolderHandler = folderHandler;
            CreateDirectoryModels();
        }

        private void CreateDirectoryModels()
        {
            Subfolders.Clear();
            foreach (var folder in FolderHandler.Subfolders)
            {
                var folderModel = new DirectoryViewModel(folder);
                folderModel.PropertyChanged += FolderModel_PropertyChanged;
                Subfolders.Add(folderModel);
            }
        }

        private void FolderModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DirectoryViewModel model;
            if (StringUtils.Equals(e.PropertyName, nameof(model.Checked)) == false) { return; }

            Checked = GetRevitFamiliesCheckedStatus();
        }

        private bool? GetRevitFamiliesCheckedStatus()
        {
            var allChecked = true;
            var allUnchecked = true;
            foreach (var family in RecursiveFamilies)
            {
                allChecked &= family.Checked == true;
                allUnchecked &= family.Checked == false;

                if (allChecked == false && allUnchecked == false) { break; }
            }

            if (allChecked == true) { return true; }
            if (allUnchecked == true) { return false; }
            return null;
        }

        private bool _Selected = false;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected == value) { return; }

                _Selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public string DirectoryName
        {
            get { return FolderHandler.Folder.Name; }
        }

        private string _FilesCount = string.Empty;
        public string FilesCount
        {
            get { return _FilesCount; }
            set
            {
                if (_FilesCount.Equals(value, StringComparison.CurrentCulture)) { return; }

                _FilesCount = value;
                OnPropertyChanged(nameof(FilesCount));
            }
        }

        public bool? Checked
        {
            get { return FolderHandler.IsSelected; }
            set
            {
                if (FolderHandler.IsSelected == value) { return; }

                FolderHandler.IsSelected = value;
                OnPropertyChanged(nameof(Checked));
            }
        }

        private Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set
            {
                if (_Visibility == value) { return; }

                _Visibility = value;
                OnPropertyChanged(nameof(Visibility));
            }
        }

        internal void UpdateCheckedFileCount(FilterManager manager)
        {
            var filtered = FolderHandler.GetFilteredRecusiveCount(manager, out var selected);
            FilesCount = $"[{selected}/{filtered}]";
        }

        public void FilterUpdated(FilterManager manager)
        {
            foreach (var child in Subfolders)
            {
                child.FilterUpdated(manager);
            }
            UpdateCheckedFileCount(manager);
        }

        public ObservableCollection<DirectoryViewModel> Subfolders { get; }
            = new ObservableCollection<DirectoryViewModel>();


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
