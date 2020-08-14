using DataSource.Model.FileSystem;
using RevitJournalUI.JournalTaskUI.FamilyFilter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.Models
{
    public class DirectoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<FamilyViewModel> RecursiveFamilies = new List<FamilyViewModel>();
        private readonly DirectoryViewModel Parent;
        private FilterManager FilterManager;

        public DirectoryViewModel(RevitDirectory directory, DirectoryViewModel parent, FilterManager filter)
        {
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }

            Parent = parent;
            FilterManager = filter;
            CreateDirectoryModels(directory);
            CreateFamilyModels(directory);
            DirectoryName = directory.Name;
        }

        private void CreateDirectoryModels(RevitDirectory directory)
        {
            SubDirectories.Clear();
            foreach (var subDirectory in directory.SubDirectories)
            {
                var model = new DirectoryViewModel(subDirectory, this, FilterManager);
                SubDirectories.Add(model);
            }
        }

        private void CreateFamilyModels(RevitDirectory directory)
        {
            FamilyViewModels.Clear();
            var libraryPath = directory.Root.FullPath;
            foreach (var revitFamily in directory.GetRevitFamilies(libraryPath, false))
            {
                var model = new FamilyViewModel(this, revitFamily);
                FamilyViewModels.Add(model);
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

        public string DirectoryName { get; private set; }

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

        private bool? _Checked = true;
        public bool? Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked == value) { return; }

                _Checked = value;
                OnPropertyChanged(nameof(Checked));

                if (Checked is null) { return; }

                UpdateFamilies((bool)Checked);
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

        private void UpdateFamilies(bool isChecked)
        {
            for (var idx = RecursiveFamilies.Count - 1; idx >= 0; idx--)
            {
                RecursiveFamilies[idx].Checked = isChecked;
            }
        }

        private void UpdateCheckedFileCount()
        {
            var filteredModels = RecursiveFamilies.Where(file => FilterManager.FileFilter(file));
            FilesCount = $"[{filteredModels.Count(file => file.Checked)}/{filteredModels.Count()}]";
        }

        public void UpdateParent()
        {
            var checkedRevitViewModels = GetRevitFamiliesCheckedStatus();
            Checked = checkedRevitViewModels;
            UpdateCheckedFileCount();
            if (Parent is null) { return; }

            Parent.UpdateParent();
        }

        public void FilterUpdated(FilterManager filter)
        {
            FilterManager = filter;
            foreach (var child in SubDirectories)
            {
                child.FilterUpdated(FilterManager);
            }
            UpdateCheckedFileCount();
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

            if(allChecked == true) { return true; }
            if(allUnchecked == true) { return false; }
            return null;
        }

        internal IList<FamilyViewModel> CreateRecursiveChildren()
        {
            RecursiveFamilies.Clear();
            RecursiveFamilies.AddRange(FamilyViewModels);
            foreach (var directory in SubDirectories)
            {
                RecursiveFamilies.AddRange(directory.CreateRecursiveChildren());
            }
            UpdateCheckedFileCount();
            return RecursiveFamilies;
        }

        public ObservableCollection<DirectoryViewModel> SubDirectories { get; }
            = new ObservableCollection<DirectoryViewModel>();

        public IList<FamilyViewModel> RecursiveFamilyViewModel { get { return RecursiveFamilies; } }

        public ObservableCollection<FamilyViewModel> FamilyViewModels { get; }
            = new ObservableCollection<FamilyViewModel>();

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
