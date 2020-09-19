using DataSource.Model.FileSystem;
using RevitJournal.Library;
using RevitJournalUI.JournalManagerUI;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using RevitJournalUI.JournalTaskUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Utilities;

namespace RevitJournalUI.JournalTaskUI
{
    public class FamilyOverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LibraryManager LibraryManager { get; set; }

        #region Revit Files

        public bool HasCheckedRevitFiles
        {
            get { return CheckedRevitFilesCount > 0; }
        }

        private int checkedRevitFilesCount = 0;
        public int CheckedRevitFilesCount
        {
            get { return checkedRevitFilesCount; }
            set
            {
                if (checkedRevitFilesCount == value) { return; }

                checkedRevitFilesCount = value;
                OnPropertyChanged(nameof(CheckedRevitFilesCount));
            }
        }

        public bool HasCheckedAndValidRevitFiles
        {
            get { return ValidCheckedRevitFilesCount > 0; }
        }

        private int validCheckedRevitFilesCount = 0;
        public int ValidCheckedRevitFilesCount
        {
            get { return validCheckedRevitFilesCount; }
            set
            {
                if (validCheckedRevitFilesCount == value) { return; }

                validCheckedRevitFilesCount = value;
                OnPropertyChanged(nameof(ValidCheckedRevitFilesCount));
            }
        }

        public bool HasEditableRevitFiles
        {
            get { return EditableRevitFilesCount > 0; }
        }

        private int editableRevitFilesCount = 0;
        public int EditableRevitFilesCount
        {
            get { return editableRevitFilesCount; }
            set
            {
                if (editableRevitFilesCount == value) { return; }

                editableRevitFilesCount = value;
                OnPropertyChanged(nameof(EditableRevitFilesCount));
            }
        }

        public ObservableCollection<DirectoryViewModel> DirectoryViewModels { get; }
            = new ObservableCollection<DirectoryViewModel>();

        public void OnContentDirectoryChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args is null || !(sender is JournalManagerPageModel model)
                || StringUtils.Equals(args.PropertyName, nameof(model.FamilyDirectory)) == false) { return; }

            UpdateDirectoryViewModels(model);
        }

        private void OnAllCheckedChanged(object sender, PropertyChangedEventArgs args)
        {
            if (IsPropertyChanged(args) == false) { return; }

            UpdateCheckedFamilyCount();
            UpdateFamilyViewModels(SelectedDirectory);
        }

        private void UpdateCheckedFamilyCount()
        {
            if (RootModel is null) { return; }

            CheckedRevitFilesCount = LibraryManager.GetCheckedFiles().Count;
            ValidCheckedRevitFilesCount = LibraryManager.GetCheckedValidFiles().Count;
            EditableRevitFilesCount = LibraryManager.GetEditableRecursiveFiles().Count;
        }

        private bool IsPropertyChanged(PropertyChangedEventArgs args)
        {
            DirectoryViewModel model;
            return args != null
                   || args.PropertyName.Equals(nameof(model.FilesCount), StringComparison.CurrentCulture);
        }

        #endregion

        #region Filtering

        public DirectoryViewModel SelectedDirectory { get; internal set; }

        public ObservableCollection<FamilyViewModel> DirectoryFiles { get; }
            = new ObservableCollection<FamilyViewModel>();

        public void UpdateFamilyViewModels(DirectoryViewModel model)
        {
            if (model is null) { return; }

            DirectoryFiles.Clear();
            foreach (var file in model.FolderHandler.Files)
            {
                var viewModel = new FamilyViewModel(file);
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
                DirectoryFiles.Add(viewModel);
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FamilyViewModel model;
            if (StringUtils.Equals(e.PropertyName, nameof(model.Checked)) == false) { return; }

            UpdateSelectedModel();
        }

        public bool FileFilter(object parameter)
        {
            if (!(parameter is FamilyViewModel model)) { return false; }

            var file = model.FileHandler.File;
            return LibraryManager.FilterManager.FileFilter(file);
        }

        public bool DirectoryFilter(object parameter)
        {
            if (!(parameter is DirectoryViewModel model)) { return false; }

            var folder = model.FolderHandler.Folder;
            return LibraryManager.FilterManager.DirectoryFilter(folder);
        }

        private DirectoryViewModel RootModel;

        public void UpdateDirectoryViewModels(JournalManagerPageModel model)
        {
            if (model is null) { return; }

            DirectoryViewModels.Clear();
            var root = LibraryManager.CreateRoot(model.TaskOptions);
            RootModel = new DirectoryViewModel(root);
            root.Setup(LibraryManager.FilterManager);
            UpdateCheckedFamilyCount();
            RootModel.UpdateCheckedFileCount(LibraryManager.FilterManager);
            RootModel.PropertyChanged += new PropertyChangedEventHandler(OnAllCheckedChanged);
            DirectoryViewModels.Add(RootModel);
        }

        private void UpdateSelectedModel()
        {
            if (SelectedDirectory is null) { return; }

            SelectedDirectory.UpdateCheckedFileCount(LibraryManager.FilterManager);
            SelectedDirectory.UpdateCheckedStatus();
        }

        public void FilterUpdated(FilterManager manager)
        {
            if (RootModel is null || manager is null) { return; }

            var selected = SelectedDirectory;
            foreach (var directory in DirectoryViewModels)
            {
                directory.FilterUpdated(manager);
            }
            DirectoryViewModels.Clear();
            DirectoryViewModels.Add(RootModel);
            UpdateCheckedFamilyCount();

            if (selected is null && RootModel is null) { return; }

            if (selected is null || selected.Visibility == Visibility.Collapsed)
            {
                selected = RootModel;
            }
            SelectedDirectory = selected;
            SelectedDirectory.Selected = true;
        }

        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
