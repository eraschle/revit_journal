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
    public class DirectoryViewModel : PathViewModel
    {
        private readonly List<FamilyViewModel> RecursiveFamilies = new List<FamilyViewModel>();
        private FilterManager FilterManager;

        public DirectoryViewModel(RevitDirectory directory, DirectoryViewModel parent, FilterManager filter) : base(parent)
        {
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }

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

        public void UpdateCheckedCount()
        {
            var filteredModels = RecursiveFamilies.Where(file => FilterManager.FileFilter(file));
            FilesCount = $"[{filteredModels.Count(file => file.Checked == true)}/{filteredModels.Count()}]";
        }

        public void FilterUpdated(FilterManager filter)
        {
            FilterManager = filter;
            foreach (var child in SubDirectories)
            {
                child.FilterUpdated(FilterManager);
            }
            UpdateCheckedCount();
        }

        protected override void UpdateChildren(bool isChecked)
        {
            base.UpdateChildren(isChecked);
            foreach (var family in FamilyViewModels)
            {
                family.SetChecked(isChecked, false, false);
            }
            foreach (var directory in SubDirectories)
            {
                directory.SetChecked(isChecked, true, false);
            }
            UpdateCheckedCount();
        }

        internal void UpdateCheckState()
        {
            bool? state = null;
            for (int idx = 0; idx < RecursiveFamilies.Count; ++idx)
            {
                bool? current = RecursiveFamilies[idx].Checked;
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
            SetChecked(state, false, true);
        }

        internal IList<FamilyViewModel> CreateRecursiveChildren()
        {
            RecursiveFamilies.Clear();
            RecursiveFamilies.AddRange(FamilyViewModels);
            foreach (var directory in SubDirectories)
            {
                RecursiveFamilies.AddRange(directory.CreateRecursiveChildren());
            }
            UpdateCheckedCount();
            return RecursiveFamilies;
        }

        public ObservableCollection<DirectoryViewModel> SubDirectories { get; }
            = new ObservableCollection<DirectoryViewModel>();

        public IList<FamilyViewModel> RecursiveFamilyViewModel { get { return RecursiveFamilies; } }

        public ObservableCollection<FamilyViewModel> FamilyViewModels { get; }
            = new ObservableCollection<FamilyViewModel>();

    }
}
