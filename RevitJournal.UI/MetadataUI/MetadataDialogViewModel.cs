using DataSource.Helper;
using DataSource.Model.Family;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Utilities;

namespace RevitJournalUI.MetadataUI
{
    public class MetadataDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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


        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != null && _Name.Equals(value, StringComparison.CurrentCulture)) { return; }

                _Name = value;
                OnPropertyChanged(nameof(Name));
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

        private string _Category = string.Empty;
        public string Category
        {
            get { return _Category; }
            set
            {
                if (_Category != null && _Category.Equals(value, StringComparison.CurrentCulture)) { return; }

                _Category = value;
                OnPropertyChanged(nameof(Category));
            }
        }


        private string _OmniClass = string.Empty;
        public string OmniClass
        {
            get { return _OmniClass; }
            set
            {
                if (_OmniClass != null && _OmniClass.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OmniClass = value;
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
                if (_SelectedFamilyType == value) { return; }

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
                FamilyTypeParameters.Add(parameter);
            }
        }

        public ObservableCollection<Parameter> FamilyParameters { get; } = new ObservableCollection<Parameter>();

        public ObservableCollection<FamilyType> FamilyTypes { get; } = new ObservableCollection<FamilyType>();

        public ObservableCollection<Parameter> FamilyTypeParameters { get; } = new ObservableCollection<Parameter>();


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
