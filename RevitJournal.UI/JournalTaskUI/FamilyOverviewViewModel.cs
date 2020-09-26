using RevitJournal.Library;
using RevitJournalUI.JournalManagerUI;
using RevitJournalUI.JournalTaskUI.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI
{
    public class FamilyOverviewViewModel : ANotifyPropertyChangedModel
    {
        public LibraryManager LibraryManager { get; set; }

        #region Revit Files

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

            UpdateFamilyViewModels(SelectedDirectory);
        }

        private bool IsPropertyChanged(PropertyChangedEventArgs args)
        {
            DirectoryViewModel model;
            return args != null
                   || args.PropertyName.Equals(nameof(model.FilesCountValue), StringComparison.CurrentCulture);
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
            foreach (var file in model.Handler.Files)
            {
                var viewModel = new FamilyViewModel(file, model);
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
                DirectoryFiles.Add(viewModel);
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FamilyViewModel model;
            if (StringUtils.Equals(e.PropertyName, nameof(model.Checked)) == false
                || SelectedDirectory is null) { return; }

            SelectedDirectory.UpdateCheckedStatus(true);
            SelectedDirectory.UpdateCheckedCount(true);
        }

        private DirectoryViewModel RootModel;

        public void UpdateDirectoryViewModels(JournalManagerPageModel model)
        {
            if (model is null) { return; }

            DirectoryViewModels.Clear();
            LibraryManager.CreateRoot(model.TaskOptions);
            LibraryManager.Root.Setup();
            RootModel = new DirectoryViewModel(LibraryManager.Root, null);
            RootModel.PropertyChanged += model.OnCheckedChanged;
            RootModel.PropertyChanged += OnAllCheckedChanged;
            DirectoryViewModels.Add(RootModel);
            model.UpdateDuplicateName();
            model.UpdateEditName();
        }

        public void FilterUpdated()
        {
            if (RootModel is null) { return; }

            var selected = SelectedDirectory;
            foreach (var directory in DirectoryViewModels)
            {
                directory.FilterUpdated();
            }
            DirectoryViewModels.Clear();
            DirectoryViewModels.Add(RootModel);

            if (selected is null && RootModel is null) { return; }

            if (selected is null)
            {
                selected = RootModel;
            }
            SelectedDirectory = selected;
        }

        #endregion
    }
}
