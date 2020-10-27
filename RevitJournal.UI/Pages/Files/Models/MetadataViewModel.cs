using DataSource.Model.Metadata;
using DataSource.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.Pages.Files.Models
{
    public class MetadataViewModel : ANotifyPropertyChangedModel
    {
        private const string Empty = "";

        private PathModel current;

        public void Update(PathModel pathModel)
        {
            if (pathModel is null) { return; }

            pathModel.PropertyChanged += Current_PropertyChanged;
            UpdateFolderData(pathModel);
            UpdateFileData(pathModel);
            if (current is object)
            {
                current.PropertyChanged -= Current_PropertyChanged;
            }
            current = pathModel;
        }

        private void UpdateFolderData(PathModel pathModel)
        {
            if (!(pathModel is FolderModel folder)) { return; }

            ShowFolderMetadata();
            FolderName = folder.Name;
            FilesCount = folder.FileCount.ToString(CultureInfo.CurrentCulture);
            ValidFilesCount = folder.ValidFileCount.ToString(CultureInfo.CurrentCulture);
            CheckedFilesCount = folder.CheckedFileCount.ToString(CultureInfo.CurrentCulture);
        }

        private void UpdateFileData(PathModel pathModel)
        {
            if (!(pathModel is FileModel file)) { return; }

            ShowFileMetadata();
            var metadata = file.GetMetadata();
            if (file.MetadataStatus != MetadataStatus.Valid
                || metadata is null)
            {
                ClearMetadata($"No DATA: Status >> {file.MetadataStatus}");
                return;
            }

            Name = metadata.Name;
            DisplayName = metadata.DisplayName;
            LibraryPath = metadata.LibraryPath;
            Updated = DateUtils.AsString(metadata.Updated);
            if (metadata.HasCategory(out var category))
            {
                Category = category.Name;
            }
            else
            {
                Category = Empty;
            }
            if (metadata.HasOmniClass(out var omniClass))
            {
                OmniClass = omniClass.NumberAndName;
            }
            else
            {
                OmniClass = Empty;
            }
            if (metadata.HasProduct(out var product))
            {
                Product = product.ProductName;
            }
            else
            {
                Product = Empty;
            }

            FamilyParameters.Clear();
            foreach (var parameter in metadata.Parameters)
            {
                FamilyParameters.Add(parameter);
            }

            FamilyTypes.Clear();
            if (metadata.FamilyTypes.Count == 0)
            {
                SelectedFamilyType = null;
                return;
            }

            foreach (var familyType in metadata.FamilyTypes)
            {
                FamilyTypes.Add(familyType);
            }
            SelectedFamilyType = FamilyTypes[0];
        }

        private void Current_PropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var propName = args.PropertyName;
            if (sender is FolderModel folder
                && (StringUtils.Equals(propName, nameof(folder.FilesCountValue))
                    || StringUtils.Equals(propName, nameof(folder.ValidFileCount))
                    )
                )
            {
                UpdateFolderData(folder);
            }
            if (sender is FileModel file
                && StringUtils.Equals(propName, nameof(file.MetadataStatus)))
            {
                UpdateFileData(file);
            }
        }

        public void ClearMetadata(string emptyValue = Empty)
        {
            Name = emptyValue;
            DisplayName = emptyValue;
            LibraryPath = emptyValue;
            Category = emptyValue;
            OmniClass = emptyValue;
            Product = emptyValue;
            Updated = emptyValue;
            FamilyParameters.Clear();
            FamilyTypes.Clear();
            SelectedFamilyType = null;

            FolderName = emptyValue;
            FilesCount = emptyValue;
            ValidFilesCount = emptyValue;
            CheckedFilesCount = emptyValue;
        }

        #region File metadata

        public void ShowFileMetadata()
        {
            FolderMetadataVisibility = Visibility.Collapsed;
            FileMetadataVisibility = Visibility.Visible;
        }

        private Visibility fileMetadataVisibility = Visibility.Collapsed;
        public Visibility FileMetadataVisibility
        {
            get { return fileMetadataVisibility; }
            set
            {
                if (fileMetadataVisibility == value) { return; }

                fileMetadataVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                if (StringUtils.Equals(name, value)) { return; }

                name = value;
                NotifyPropertyChanged();
            }
        }

        private string displayName = string.Empty;
        public string DisplayName
        {
            get { return displayName; }
            set
            {
                if (StringUtils.Equals(displayName, value)) { return; }

                displayName = value;
                NotifyPropertyChanged();
            }
        }

        private string libraryPath = string.Empty;
        public string LibraryPath
        {
            get { return libraryPath; }
            set
            {
                if (StringUtils.Equals(libraryPath, value)) { return; }

                libraryPath = value;
                NotifyPropertyChanged();
            }
        }

        private string category = string.Empty;
        public string Category
        {
            get { return category; }
            set
            {
                if (StringUtils.Equals(category, value)) { return; }

                category = value;
                NotifyPropertyChanged();
            }
        }


        private string omniClass = string.Empty;
        public string OmniClass
        {
            get { return omniClass; }
            set
            {
                if (StringUtils.Equals(omniClass, value)) { return; }

                omniClass = value;
                NotifyPropertyChanged();
            }
        }

        private string updated = string.Empty;
        public string Updated
        {
            get { return updated; }
            set
            {
                if (StringUtils.Equals(updated, value)) { return; }

                updated = value;
                NotifyPropertyChanged();
            }
        }

        private string product = string.Empty;
        public string Product
        {
            get { return product; }
            set
            {
                if (StringUtils.Equals(product, value)) { return; }

                product = value;
                NotifyPropertyChanged();
            }
        }

        private FamilyType selectedFamilyType;
        public FamilyType SelectedFamilyType
        {
            get { return selectedFamilyType; }
            set
            {
                if (selectedFamilyType == value) { return; }

                selectedFamilyType = value;
                NotifyPropertyChanged();
                UpdateFamilyTypeParameters();
            }
        }

        private void UpdateFamilyTypeParameters()
        {
            FamilyTypeParameters.Clear();
            if (SelectedFamilyType is null) { return; }

            foreach (var parameter in SelectedFamilyType.Parameters)
            {
                FamilyTypeParameters.Add(parameter);
            }
        }

        public ObservableCollection<Parameter> FamilyParameters { get; } = new ObservableCollection<Parameter>();

        public ObservableCollection<FamilyType> FamilyTypes { get; } = new ObservableCollection<FamilyType>();

        public ObservableCollection<Parameter> FamilyTypeParameters { get; } = new ObservableCollection<Parameter>();

        #endregion

        #region Folder metadata

        public void ShowFolderMetadata()
        {
            FileMetadataVisibility = Visibility.Collapsed;
            FolderMetadataVisibility = Visibility.Visible;
        }

        private Visibility folderMetadataVisibility = Visibility.Collapsed;
        public Visibility FolderMetadataVisibility
        {
            get { return folderMetadataVisibility; }
            set
            {
                if (folderMetadataVisibility == value) { return; }

                folderMetadataVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private string folderName = string.Empty;
        public string FolderName
        {
            get { return folderName; }
            set
            {
                if (StringUtils.Equals(folderName, value)) { return; }

                folderName = value;
                NotifyPropertyChanged();
            }
        }

        private string filesCount = string.Empty;
        public string FilesCount
        {
            get { return filesCount; }
            set
            {
                if (StringUtils.Equals(filesCount, value)) { return; }

                filesCount = value;
                NotifyPropertyChanged();
            }
        }

        private string validFilesCount = string.Empty;
        public string ValidFilesCount
        {
            get { return validFilesCount; }
            set
            {
                if (StringUtils.Equals(validFilesCount, value)) { return; }

                validFilesCount = value;
                NotifyPropertyChanged();
            }
        }

        private string checkedFilesCount = string.Empty;
        public string CheckedFilesCount
        {
            get { return checkedFilesCount; }
            set
            {
                if (StringUtils.Equals(checkedFilesCount, value)) { return; }

                checkedFilesCount = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}