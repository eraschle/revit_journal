using DataSource.Model.FileSystem;
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

        public FilterManager FilterManager { get; set; }

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

        public IList<RevitFamily> RecursiveRevitFamilies
        {
            get
            {
                if (RootModel is null) { return new List<RevitFamily>(); }

                return RootModel.RecursiveFamilyViewModel
                                .Where(model => IsFilteredAndCheckedModel(model))
                                .Select(model => model.RevitFamily).ToList();
            }
        }

        public IList<RevitFamily> ValidRecursiveRevitFamilies
        {
            get
            {
                return RecursiveRevitFamilies.Where(model => model.HasValidMetadata)
                                             .ToList();
            }
        }

        public IList<RevitFamily> EditableRecursiveRevitFamilies
        {
            get
            {
                return ValidRecursiveRevitFamilies
                    .Where(model => model.HasFileMetadata)
                    .ToList();
            }
        }

        private bool IsFilteredAndCheckedModel(FamilyViewModel model)
        {
            return model.Checked && FilterManager.FileFilter(model);
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

            CheckedRevitFilesCount = RecursiveRevitFamilies.Count;
            ValidCheckedRevitFilesCount = ValidRecursiveRevitFamilies.Count;
            EditableRevitFilesCount = EditableRecursiveRevitFamilies.Count;
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

        public ObservableCollection<FamilyViewModel> DirectoryFamilies { get; }
            = new ObservableCollection<FamilyViewModel>();

        public void UpdateFamilyViewModels(DirectoryViewModel model)
        {
            if (model is null) { return; }

            DirectoryFamilies.Clear();
            foreach (var revitFile in model.FamilyViewModels)
            {
                DirectoryFamilies.Add(revitFile);
            }
        }

        public bool FileFilter(object parameter)
        {
            if (!(parameter is FamilyViewModel model)) { return false; }
            if (FilterManager is null) { return true; }

            return FilterManager.FileFilter(model);
        }

        public bool DirectoryFilter(object parameter)
        {
            if (!(parameter is DirectoryViewModel model)) { return false; }
            if (FilterManager is null) { return true; }

            return FilterManager.DirectoryFilter(model);
        }

        private DirectoryViewModel RootModel;

        public void UpdateDirectoryViewModels(JournalManagerPageModel model)
        {
            if (model is null) { return; }

            DirectoryViewModels.Clear();
            var root = new RevitDirectory(null, model.TaskOptions.RootDirectory);
            RootModel = new DirectoryViewModel(root, null, FilterManager);
            RootModel.CreateRecursiveChildren();
            UpdateCheckedFamilyCount();
            RootModel.PropertyChanged += new PropertyChangedEventHandler(OnAllCheckedChanged);
            DirectoryViewModels.Add(RootModel);
        }

        public void FilterUpdated(FilterManager filter)
        {
            if (RootModel is null || filter is null) { return; }

            FilterManager = filter;
            var selected = SelectedDirectory;
            foreach (var directory in DirectoryViewModels)
            {
                directory.FilterUpdated(filter);
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
