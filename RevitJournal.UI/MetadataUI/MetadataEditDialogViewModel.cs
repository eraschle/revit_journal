using DataSource;
using DataSource.Helper;
using DataSource.Model.Catalog;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using DataSource.Model.ProductData;
using RevitJournalUI.MetadataUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Utilities.System;

namespace RevitJournalUI.MetadataUI
{
    public class MetadataEditDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
            return family.Name.Equals(other.Name, StringComparison.CurrentCulture)
                && family.LibraryPath.Equals(other.LibraryPath, StringComparison.CurrentCulture);
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
                OnPropertyChanged(nameof(Category));
            }

            if (Family.HasOmniClass(out _))
            {
                OnPropertyChanged(nameof(OmniClass));
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


        private string _FamilyName = string.Empty;
        public string FamilyName
        {
            get { return _FamilyName; }
            set
            {
                if (_FamilyName != null && _FamilyName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _FamilyName = value;
                OnPropertyChanged(nameof(FamilyName));
            }
        }

        private string _DisplayName = string.Empty;
        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                if (_DisplayName != null && _DisplayName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private string _LibraryPath = string.Empty;
        public string LibraryPath
        {
            get { return _LibraryPath; }
            set
            {
                if (_LibraryPath != null && _LibraryPath.Equals(value, StringComparison.CurrentCulture)) { return; }

                _LibraryPath = value;
                OnPropertyChanged(nameof(LibraryPath));
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
                OnPropertyChanged(nameof(Category));
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
                OnPropertyChanged(nameof(OmniClass));
            }
        }

        private string _Updated = string.Empty;
        public string Updated
        {
            get { return _Updated; }
            set
            {
                if (_Updated != null && _Updated.Equals(value, StringComparison.CurrentCulture)) { return; }

                _Updated = value;
                OnPropertyChanged(nameof(Updated));
            }
        }

        private string _Product = string.Empty;
        public string Product
        {
            get { return _Product; }
            set
            {
                if (_Product != null && _Product.Equals(value, StringComparison.CurrentCulture)) { return; }

                _Product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        private FamilyType _SelectedFamilyType;
        public FamilyType SelectedFamilyType
        {
            get { return _SelectedFamilyType; }
            set
            {
                if (value is null ||
                    _SelectedFamilyType != null &&
                    _SelectedFamilyType.Equals(value)) { return; }

                _SelectedFamilyType = value;
                OnPropertyChanged(nameof(SelectedFamilyType));
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

        public ObservableCollection<ParameterFileMetadataViewModel> FamilyParameters { get; } = new ObservableCollection<ParameterFileMetadataViewModel>();

        public ObservableCollection<FamilyType> FamilyTypes { get; } = new ObservableCollection<FamilyType>();

        public ObservableCollection<ParameterFileMetadataViewModel> FamilyTypeParameters { get; } = new ObservableCollection<ParameterFileMetadataViewModel>();


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
