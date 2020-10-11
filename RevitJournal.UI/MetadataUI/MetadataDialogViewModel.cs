using DataSource.Model.Metadata;
using System.Collections.ObjectModel;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.MetadataUI
{
    public class MetadataDialogViewModel : ANotifyPropertyChangedModel
    {
        public void UpdateFamily(Family family)
        {
            if (family is null) { return; }

            Name = family.Name;
            DisplayName = family.DisplayName;
            LibraryPath = family.LibraryPath;
            if (family.HasCategory(out var category))
            {
                Category = category.Name;
            }
            if (family.HasOmniClass(out var omniClass))
            {
                OmniClass = omniClass.NumberAndName;
            }
            if (family.HasProduct(out var product))
            {
                Product = product.ProductName;
            }
            Updated = DateUtils.AsString(family.Updated);

            FamilyParameters.Clear();
            foreach (var parameter in family.Parameters)
            {
                FamilyParameters.Add(parameter);
            }
            FamilyTypes.Clear();

            if (family.FamilyTypes.Count == 0) { return; }

            foreach (var familyType in family.FamilyTypes)
            {
                FamilyTypes.Add(familyType);
            }

            SelectedFamilyType = FamilyTypes[0];
        }


        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != null && StringUtils.Equals(name, value)) { return; }

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
                if (displayName != null && StringUtils.Equals(displayName, value)) { return; }

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
                if (libraryPath != null && StringUtils.Equals(libraryPath, value)) { return; }

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
                if (category != null && StringUtils.Equals(category, value)) { return; }

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
                if (omniClass != null && StringUtils.Equals(omniClass, value)) { return; }

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
                if (updated != null && StringUtils.Equals(updated, value)) { return; }

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
                if (product != null && StringUtils.Equals(product, value)) { return; }

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
            foreach (var parameter in SelectedFamilyType.Parameters)
            {
                FamilyTypeParameters.Add(parameter);
            }
        }

        public ObservableCollection<Parameter> FamilyParameters { get; } = new ObservableCollection<Parameter>();

        public ObservableCollection<FamilyType> FamilyTypes { get; } = new ObservableCollection<FamilyType>();

        public ObservableCollection<Parameter> FamilyTypeParameters { get; } = new ObservableCollection<Parameter>();
    }
}
