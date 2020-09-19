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

        private readonly DirectoryViewModel parent;

        internal SelectFolderHandler FolderHandler { get; private set; }

        public DirectoryViewModel(SelectFolderHandler folderHandler, DirectoryViewModel parentViewModel)
        {
            if (folderHandler is null) { throw new ArgumentNullException(nameof(folderHandler)); }

            FolderHandler = folderHandler;
            parent = parentViewModel;
            CreateDirectoryModels();
        }

        private void CreateDirectoryModels()
        {
            Subfolders.Clear();
            foreach (var folder in FolderHandler.Subfolders)
            {
                var folderModel = new DirectoryViewModel(folder, this);
                Subfolders.Add(folderModel);
            }
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
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == FolderHandler.IsSelected) { return; }

            FolderHandler.IsSelected = value;

            if (updateChildren && FolderHandler.IsSelected.HasValue)
            {
                foreach (var folder in Subfolders)
                {
                    folder.SetIsChecked(value, true, false);
                }
            }

            if (updateParent && parent != null)
            {
                parent.VerifyCheckState();
            }

            OnPropertyChanged(nameof(Checked));
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int idx = 0; idx < FolderHandler.RecusiveFiles.Count; ++idx)
            {
                bool? current = FolderHandler.RecusiveFiles[idx].IsSelected;
                if (idx == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetIsChecked(state, false, true);
        }


        internal void UpdateCheckedStatus()
        {
            var allChecked = true;
            var allUnchecked = true;
            foreach (var family in FolderHandler.RecusiveFiles)
            {
                allChecked &= family.IsSelected == true;
                allUnchecked &= family.IsSelected == false;

                if (allChecked == false && allUnchecked == false) { break; }
            }

            if (allChecked == true) { Checked = true; }
            if (allUnchecked == true) { Checked = false; }
            Checked = null;
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
