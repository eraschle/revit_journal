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
    public class DirectoryViewModel : PathViewModel<LibraryFolder>
    {
        public DirectoryViewModel(LibraryFolder handler, DirectoryViewModel parent): base(handler, parent)
        {
            CreateDirectoryModels();
        }

        private void CreateDirectoryModels()
        {
            Subfolders.Clear();
            foreach (var folder in Handler.Subfolders)
            {
                var folderModel = new DirectoryViewModel(folder, this);
                Subfolders.Add(folderModel);
            }
            UpdateCheckedCount(false);
        }

        public string DirectoryName
        {
            get { return Handler.Folder.Name; }
        }

        private int filteredCount = 0;
        public int FilteredCount
        {
            get { return filteredCount; }
            set
            {
                if (filteredCount == value) { return; }

                filteredCount = value;
                OnPropertyChanged(nameof(FilteredCount));
            }
        }

        private string filesCountValue = string.Empty;
        public string FilesCountValue
        {
            get { return filesCountValue; }
            set
            {
                if (filesCountValue.Equals(value, StringComparison.CurrentCulture)) { return; }

                filesCountValue = value;
                OnPropertyChanged(nameof(FilesCountValue));
            }
        }

        protected override void FinishSetChecked()
        {
            base.FinishSetChecked();
            UpdateCheckedCount(true);
        }

        protected override void UpdateChildren()
        {
            base.UpdateChildren();
            if(Handler.IsChecked.HasValue == false) { return; }

            foreach (var folder in Subfolders)
            {
                folder.UpdateChlidrenChecked();
            }
        }

        internal void UpdateChlidrenChecked()
        {
            foreach (var folder in Subfolders)
            {
                folder.UpdateChlidrenChecked();
            }
            UpdateChecked();
            UpdateCheckedCount(false);
        }

        public void UpdateCheckedCount(bool updateParent)
        {
            FilteredCount = Handler.FilteredCount;
            FilesCountValue = $"[{Handler.CheckedCount}/{FilteredCount}]";
            if (updateParent && Parent != null)
            {
                Parent.UpdateCheckedCount(updateParent);
            }
        }

        internal void UpdateCheckedStatus(bool updateParent)
        {
            UpdateChecked();
            if (updateParent && Parent != null)
            {
                Parent.UpdateCheckedStatus(updateParent);
            }
        }

        public void FilterUpdated()
        {
            foreach (var child in Subfolders)
            {
                child.FilterUpdated();
            }
            Handler.UpdateFileCounts();
            UpdateCheckedCount(false);
        }

        public ObservableCollection<DirectoryViewModel> Subfolders { get; }
            = new ObservableCollection<DirectoryViewModel>();


    }
}
