using DataSource;
using DataSource.Model.Catalog;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using DataSource.Model.ProductData;
using RevitJournalUI.MetadataUI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.MetadataUI
{
    public class MetadataEditDialogViewModel : ANotifyPropertyChangedModel
    {
        private IList<RevitFamily> RevitFamilies = new List<RevitFamily>();

        private ProductDataManager ProductDataManager = ProductDataManager.Get();
        private RevitProductData ProductData;

        public RevitFamily GetRevitFamily(Family family)
        {
            RevitFamily reloaded = null;
            if (family != null)
            {
                foreach (var rvtFamily in RevitFamilies)
                {
                    if (IsFamily(rvtFamily.Metadata, family) == false) { continue; }

                    reloaded = rvtFamily;
                    break;
                }
            }

            return reloaded;
        }

        private static bool IsFamily(Family family, Family other)
        {
            return StringUtils.Equals(family.Name, other.Name)
                && StringUtils.Equals(family.LibraryPath, other.LibraryPath);
        }

        public ObservableCollection<Family> Families { get; } = new ObservableCollection<Family>();

        public void Update(IList<RevitFamily> families)
        {
            if (families is null) { return; }

            RevitFamilies = families;

            Families.Clear();
            foreach (var family in families)
            {
                Families.Add(family.Metadata);
            }
            Update(Families[0]);
        }

        private Family Family;

        public void Update(Family family)
        {
            if (family is null) { return; }

            Family = family;
            FamilyName = Family.Name;
            DisplayName = Family.DisplayName;
            LibraryPath = Family.LibraryPath;

            var familyProduct = Family.Product;
            if (ProductData is null || ProductData.Version != familyProduct.Version)
            {
                ProductData = ProductDataManager.ByVersion(familyProduct);
                UpdateCategories(ProductData.Categories);
                UpdateOmniClasses(ProductData.OmniClasses);
            }
            if (Family.HasCategory())
            {
                NotifyPropertyChanged(nameof(Category));
            }

            if (Family.HasOmniClass(out _))
            {
                NotifyPropertyChanged(nameof(OmniClass));
            }

            if (Family.HasProduct(out var product))
            {
                Product = product.ProductName;
            }

            Updated = DateUtils.AsString(Family.Updated);

            FamilyParameters.Clear();
            foreach (var parameter in Family.Parameters)
            {
                var model = GetModel(parameter);
                FamilyParameters.Add(model);
            }
            FamilyTypes.Clear();

            if (Family.FamilyTypes.Count == 0) { return; }

            foreach (var familyType in Family.FamilyTypes)
            {
                FamilyTypes.Add(familyType);
            }

            SelectedFamilyType = FamilyTypes[0];
        }


        private string familyName = string.Empty;
        public string FamilyName
        {
            get { return familyName; }
            set
            {
                if (familyName != null && StringUtils.Equals(familyName, value)) { return; }

                familyName = value;
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

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();

        private void UpdateCategories(IList<Category> categories)
        {
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        public Category Category
        {
            get
            {
                if (Family is null) { return null; }

                return Family.Category;
            }
            set
            {
                if (value is null || Family is null || Family.Category == value) { return; }

                Family.Category = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<OmniClass> OmniClasses { get; } = new ObservableCollection<OmniClass>();

        private void UpdateOmniClasses(IList<OmniClass> omniClasses)
        {
            OmniClasses.Clear();
            foreach (var omniClass in omniClasses)
            {
                OmniClasses.Add(omniClass);
            }
        }

        public OmniClass OmniClass
        {
            get
            {
                if (Family is null) { return null; }

                return Family.OmniClass;
            }
            set
            {
                if (value is null || Family is null || Family.OmniClass == value) { return; }

                Family.OmniClass = value;
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
                if (value is null ||
                    selectedFamilyType != null &&
                    selectedFamilyType.Equals(value)) { return; }

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
                var model = GetModel(parameter);
                FamilyTypeParameters.Add(model);
            }
        }

        private static ParameterFileMetadataViewModel GetModel(Parameter parameter)
        {
            return new ParameterFileMetadataViewModel { Parameter = parameter };
        }

        public ObservableCollection<ParameterFileMetadataViewModel> FamilyParameters { get; }
            = new ObservableCollection<ParameterFileMetadataViewModel>();

        public ObservableCollection<FamilyType> FamilyTypes { get; } = new ObservableCollection<FamilyType>();

        public ObservableCollection<ParameterFileMetadataViewModel> FamilyTypeParameters { get; }
            = new ObservableCollection<ParameterFileMetadataViewModel>();
    }
}
