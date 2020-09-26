using DataSource.Helper;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using RevitJournal.Duplicate;
using RevitJournal.Duplicate.Comparer;
using RevitJournal.Duplicate.Comparer.FamilyComparer;
using RevitJournal.Duplicate.Comparer.FamilyTypeComparer;
using RevitJournal.Library;
using RevitJournalUI.MetadataUI.Comparer;
using RevitJournalUI.MetadataUI.Models;
using RevitJournalUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Utilities;
using Utilities.UI.Helper;

namespace RevitJournalUI.MetadataUI
{
    public class MetadataDuplicateDialogModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly FamilyDuplicateComparer FamilyComparer = new FamilyDuplicateComparer();
        private LibraryManager libraryManager;

        public MetadataDuplicateDialogModel()
        {
            UpdateFamilyInformationComparers();
            UpdateFamilyParametersComparers();
            UpdateFamilyTypeComparers();
            UpdateFamilyTypeParametersComparers();
            DeselectAllCommand = new RelayCommand<IEnumerable<ACheckedViewModel>>(DeselectAllCommandAction, DeselectAllCommandPredicate);
            SelectAllCommand = new RelayCommand<IEnumerable<ACheckedViewModel>>(SelectAllCommandAction, SelectAllCommandPredicate);
            UseFilterCommand = new RelayCommand<object>(UseFilterCommandAction, UseFilterCommandPredicate);
        }

        public void SetRevitFiles(LibraryManager manager)
        {
            if (manager is null) { throw new ArgumentNullException(nameof(manager)); }

            libraryManager = manager;
        }

        private void CreateDupublicatedModels()
        {
            Models.Clear();
            var dublicatedMap = DuplicateManager.Create(libraryManager, FamilyComparer);
            foreach (var metadataKey in dublicatedMap.Keys)
            {
                var dublicated = dublicatedMap[metadataKey];
                if (dublicated.Count == 0) { continue; }

                var viewModel = new OriginalFamilyViewModel(metadataKey, dublicated, FamilyComparer);
                Models.Add(viewModel);
            }
        }

        #region Filter settings

        public ObservableCollection<FamilyComparerViewModel> FamilyInformations { get; }
            = new ObservableCollection<FamilyComparerViewModel>();

        public ICollection<IDuplicateComparer<Family>> FamilyInformationsComparers
        {
            get { return GetModelComparer(FamilyInformations); }
        }


        public void UpdateFamilyInformationComparers()
        {
            FamilyInformations.Clear();
            foreach (var comparer in FamilyComparer.PropertyComparers)
            {
                var model = new FamilyComparerViewModel(comparer);
                FamilyInformations.Add(model);
            }
        }

        public ObservableCollection<ParameterComparerViewModel> FamilyParameters { get; }
            = new ObservableCollection<ParameterComparerViewModel>();

        public ICollection<IDuplicateComparer<Parameter>> FamilyParameterComparers
        {
            get { return GetModelComparer(FamilyParameters); }
        }

        public void UpdateFamilyParametersComparers()
        {
            FamilyParameters.Clear();
            var parameterComparer = FamilyParametersDublicateComparer.ParameterComparer;
            foreach (var comparer in parameterComparer.PropertyComparers)
            {
                var model = new ParameterComparerViewModel(comparer);
                FamilyParameters.Add(model);
            }
        }

        public ObservableCollection<FamilyTypeComparerViewModel> FamilyTypes { get; }
            = new ObservableCollection<FamilyTypeComparerViewModel>();

        public ICollection<IDuplicateComparer<FamilyType>> FamilyTypeComparers
        {
            get { return GetModelComparer(FamilyTypes); }
        }

        public void UpdateFamilyTypeComparers()
        {
            FamilyTypes.Clear();
            foreach (var comparer in FamilyTypeDuplicateComparer.AllComparers())
            {
                var model = new FamilyTypeComparerViewModel(comparer);
                FamilyTypes.Add(model);
            }
        }

        public ObservableCollection<ParameterComparerViewModel> FamilyTypeParameters { get; }
            = new ObservableCollection<ParameterComparerViewModel>();

        public ICollection<IDuplicateComparer<Parameter>> FamilyTypeParametersComparers
        {
            get { return GetModelComparer(FamilyTypeParameters); }
        }

        private ICollection<IDuplicateComparer<TModel>> GetModelComparer<TModel>(IEnumerable<AMetadataComparerViewModel<TModel>> models)
        {
            return models.Select(model => model.Comparer).ToList();
        }

        public void UpdateFamilyTypeParametersComparers()
        {
            FamilyTypeParameters.Clear();
            var paramterComparer = FamilyTypeParametersDublicateComparer.ParameterComparer;
            foreach (var comparer in paramterComparer.PropertyComparers)
            {
                var model = new ParameterComparerViewModel(comparer);
                FamilyTypeParameters.Add(model);
            }
        }

        public ICommand DeselectAllCommand { get; }

        private bool DeselectAllCommandPredicate(IEnumerable<ACheckedViewModel> models)
        {
            return models != null && models.Any(model => model.Checked);
        }

        private void DeselectAllCommandAction(IEnumerable<ACheckedViewModel> models)
        {
            foreach (var model in models)
            {
                model.Checked = false;
            }
        }

        public ICommand SelectAllCommand { get; }

        private bool SelectAllCommandPredicate(IEnumerable<ACheckedViewModel> models)
        {
            return models != null && models.Any(model => model.Checked == false);
        }

        private void SelectAllCommandAction(IEnumerable<ACheckedViewModel> models)
        {
            foreach (var model in models)
            {
                model.Checked = true;
            }
        }

        public ICommand UseFilterCommand { get; }

        private bool UseFilterCommandPredicate(object models)
        {
            return FamilyInformationsComparers.Any(cmp => cmp.UseComparer)
                || FamilyParameterComparers.Any(cmp => cmp.UseComparer)
                || FamilyTypeComparers.Any(cmp => cmp.UseComparer)
                || FamilyTypeParametersComparers.Any(cmp => cmp.UseComparer);
        }

        private void UseFilterCommandAction(object models)
        {
            FamilyComparer.PropertyComparers = FamilyInformationsComparers;

            Family model;
            if (FamilyComparer.HasByName(nameof(model.Parameters), out var paramsComparer))
            {
                if (paramsComparer is FamilyParametersDublicateComparer familyComparer)
                {
                    var parameterComparer = new ParameterDuplicateComparer(FamilyParameterComparers);
                    familyComparer.ItemComparer = parameterComparer;
                }
            }
            if (FamilyComparer.HasByName(nameof(model.FamilyTypes), out var typesComparer))
            {
                if (typesComparer is FamilyFamilyTypesDublicateComparer familyTypesComparer)
                {
                    var familyTypeComparer = new FamilyTypeDuplicateComparer(FamilyTypeComparers);
                    FamilyType familyTypeModel;
                    if (familyTypeComparer.HasByName(nameof(familyTypeModel.Parameters), out var typeParamsComparer))
                    {
                        if (typeParamsComparer is FamilyTypeParametersDublicateComparer familyTypeParametersComparer)
                        {
                            var typeParameterComparer = new ParameterDuplicateComparer(FamilyTypeParametersComparers);
                            familyTypeParametersComparer.ItemComparer = typeParameterComparer;
                        }
                    }
                    familyTypesComparer.ItemComparer = familyTypeComparer;
                }
            }
            CreateDupublicatedModels();
            IsSearchOptionExpanded = false;
        }

        private bool _IsSearchOptionExpanded = true;
        public bool IsSearchOptionExpanded
        {
            get { return _IsSearchOptionExpanded; }
            set
            {
                if (_IsSearchOptionExpanded == value) { return; }

                _IsSearchOptionExpanded = value;
                OnPropertyChanged(nameof(IsSearchOptionExpanded));
            }
        }

        #endregion

        public ObservableCollection<OriginalFamilyViewModel> Models { get; }
            = new ObservableCollection<OriginalFamilyViewModel>();


        private OriginalFamilyViewModel _SelectedModel;
        public OriginalFamilyViewModel SelectedModel
        {
            get { return _SelectedModel; }
            set
            {
                if (_SelectedModel != null && _SelectedModel.Equals(value)) { return; }

                _SelectedModel = value;
                OnPropertyChanged(nameof(SelectedModel));
            }
        }

        public ObservableCollection<DuplicateFamilyViewModel> Duplicated { get; }
            = new ObservableCollection<DuplicateFamilyViewModel>();


        #region Original Metadata

        private Family _Original;
        public Family Original
        {
            get { return _Original; }
            set
            {
                if (_Original != null && _Original.Equals(value)) { return; }

                _Original = value;
                OnPropertyChanged(nameof(Original));
            }
        }

        public void UpdateOriginal(Family original)
        {
            if (original is null) { return; }

            Original = original;

            OriginalName = Original.Name;
            OriginalDisplayName = Original.DisplayName;
            OriginalLibraryPath = Original.LibraryPath;
            if (Original.HasCategory(out var category))
            {
                OriginalCategory = category.Name;
            }
            if (Original.HasOmniClass(out var omniClass))
            {
                OriginalOmniClass = omniClass.NumberAndName;
            }
            if (Original.HasProduct(out var product))
            {
                OriginalProduct = product.ProductName;
            }
            OriginalUpdated = DateUtils.AsString(Original.Updated);

            OriginalFamilyParameters.Clear();
            foreach (var parameter in Original.Parameters)
            {
                OriginalFamilyParameters.Add(parameter);
            }
            OriginalFamilyTypes.Clear();

            if (Original.FamilyTypes.Count == 0) { return; }

            foreach (var familyType in Original.FamilyTypes)
            {
                OriginalFamilyTypes.Add(familyType);
            }

            OriginalSelectedFamilyType = OriginalFamilyTypes[0];
        }


        private string _OriginalName = string.Empty;
        public string OriginalName
        {
            get { return _OriginalName; }
            set
            {
                if (_OriginalName != null && _OriginalName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OriginalName = value;
                OnPropertyChanged(nameof(OriginalName));
            }
        }

        private string _OriginalDisplayName = string.Empty;
        public string OriginalDisplayName
        {
            get { return _OriginalDisplayName; }
            set
            {
                if (_OriginalDisplayName != null && _OriginalDisplayName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OriginalDisplayName = value;
                OnPropertyChanged(nameof(OriginalDisplayName));
            }
        }

        private string _OriginalLibraryPath = string.Empty;
        public string OriginalLibraryPath
        {
            get { return _OriginalLibraryPath; }
            set
            {
                if (_OriginalLibraryPath != null && _OriginalLibraryPath.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OriginalLibraryPath = value;
                OnPropertyChanged(nameof(OriginalLibraryPath));
            }
        }

        private string _OriginalCategory = string.Empty;
        public string OriginalCategory
        {
            get { return _OriginalCategory; }
            set
            {
                if (_OriginalCategory != null && _OriginalCategory.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OriginalCategory = value;
                OnPropertyChanged(nameof(OriginalCategory));
            }
        }

        private string _OriginalOmniClass = string.Empty;
        public string OriginalOmniClass
        {
            get { return _OriginalOmniClass; }
            set
            {
                if (_OriginalOmniClass != null && _OriginalOmniClass.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OriginalOmniClass = value;
                OnPropertyChanged(nameof(OriginalOmniClass));
            }
        }

        private string _OriginalUpdated = string.Empty;
        public string OriginalUpdated
        {
            get { return _OriginalUpdated; }
            set
            {
                if (_OriginalUpdated != null && _OriginalUpdated.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OriginalUpdated = value;
                OnPropertyChanged(nameof(OriginalUpdated));
            }
        }

        private string _OriginalProduct = string.Empty;
        public string OriginalProduct
        {
            get { return _OriginalProduct; }
            set
            {
                if (_OriginalProduct != null && _OriginalProduct.Equals(value, StringComparison.CurrentCulture)) { return; }

                _OriginalProduct = value;
                OnPropertyChanged(nameof(OriginalProduct));
            }
        }

        private FamilyType _OriginalSelectedFamilyType;
        public FamilyType OriginalSelectedFamilyType
        {
            get { return _OriginalSelectedFamilyType; }
            set
            {
                if (value is null ||
                    _OriginalSelectedFamilyType != null &&
                    _OriginalSelectedFamilyType.Equals(value)) { return; }

                _OriginalSelectedFamilyType = value;
                OnPropertyChanged(nameof(OriginalSelectedFamilyType));
                UpdateOriginalFamilyTypeParameters();
                UpdateSourceFamilyTypeParameterDistances();
            }
        }

        private void UpdateOriginalFamilyTypeParameters()
        {
            OriginalFamilyTypeParameters.Clear();
            foreach (var parameter in OriginalSelectedFamilyType.Parameters)
            {
                OriginalFamilyTypeParameters.Add(parameter);
            }
        }

        public ObservableCollection<Parameter> OriginalFamilyParameters { get; } = new ObservableCollection<Parameter>();

        public ObservableCollection<FamilyType> OriginalFamilyTypes { get; } = new ObservableCollection<FamilyType>();

        public ObservableCollection<Parameter> OriginalFamilyTypeParameters { get; } = new ObservableCollection<Parameter>();

        #endregion

        #region Source Metadata

        private Family _Source;
        public Family Source
        {
            get { return _Source; }
            set
            {
                if (_Source != null && _Source.Equals(value)) { return; }

                _Source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public void UpdateSource(Family source)
        {
            if (source is null || Original is null) { return; }

            Source = source;

            SourceLevenstein = FamilyComparer.LevenstheinDistanceAsString(Original, Source);

            SourceDisplayName = Source.DisplayName;
            SourceDisplayNameDistance = GetDistance(nameof(Source.DisplayName));

            if (Source.HasCategory(out var category))
            {
                SourceCategory = category.Name;
                SourceCategoryDistance = GetDistance(nameof(Source.Category));
            }

            if (Source.HasOmniClass(out var omniClass))
            {
                SourceOmniClass = omniClass.NumberAndName;
                SourceOmniClassDistance = GetDistance(nameof(Source.OmniClass));
            }
            if (Source.HasProduct(out var product))
            {
                SourceProduct = product.ProductName;
                SourceProductDistance = GetDistance(nameof(Source.Product.ProductName));
            }

            SourceUpdated = DateUtils.AsString(Source.Updated);
            SourceUpdatedDistance = GetDistance(nameof(Source.Updated));

            SourceLibraryPath = Source.LibraryPath;
            SourceLibraryPathDistance = GetDistance(nameof(Source.LibraryPath));

            SourceFamilyParameters.Clear();
            var comparer = FamilyParametersDublicateComparer.ParameterComparer;
            foreach (var parameter in Source.Parameters)
            {
                var viewModel = new ParameterViewModel { Parameter = parameter };
                var originalParameter = Original.ByName(parameter.Name);
                UpdateParameterDistance(viewModel, originalParameter, comparer);
                SourceFamilyParameters.Add(viewModel);
            }

            if (Source.FamilyTypes.Count == 0) { return; }

            SourceFamilyTypes.Clear();
            foreach (var familyType in Source.FamilyTypes)
            {
                SourceFamilyTypes.Add(familyType);
            }
            SourceSelectedFamilyType = SourceFamilyTypes[0];
        }

        private string _SourceLevenstein = string.Empty;
        public string SourceLevenstein
        {
            get { return _SourceLevenstein; }
            set
            {
                if (_SourceLevenstein != null && _SourceLevenstein.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceLevenstein = value;
                OnPropertyChanged(nameof(SourceLevenstein));
            }
        }


        private string _SourceName = string.Empty;
        public string SourceName
        {
            get { return _SourceName; }
            set
            {
                if (_SourceName != null && _SourceName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceName = value;
                OnPropertyChanged(nameof(SourceName));
            }
        }


        private string _SourceNameDistance = string.Empty;
        public string SourceNameDistance
        {
            get { return _SourceNameDistance; }
            set
            {
                if (_SourceNameDistance == value) { return; }

                _SourceNameDistance = value;
                OnPropertyChanged(nameof(SourceNameDistance));
            }
        }


        private string _SourceDisplayName = string.Empty;
        public string SourceDisplayName
        {
            get { return _SourceDisplayName; }
            set
            {
                if (_SourceDisplayName != null && _SourceDisplayName.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceDisplayName = value;
                OnPropertyChanged(nameof(SourceDisplayName));
            }
        }


        private string _SourceDisplayNameDistance = string.Empty;
        public string SourceDisplayNameDistance
        {
            get { return _SourceDisplayNameDistance; }
            set
            {
                if (_SourceDisplayNameDistance == value) { return; }

                _SourceDisplayNameDistance = value;
                OnPropertyChanged(nameof(SourceDisplayNameDistance));
            }
        }


        private string _SourceLibraryPath = string.Empty;
        public string SourceLibraryPath
        {
            get { return _SourceLibraryPath; }
            set
            {
                if (_SourceLibraryPath != null && _SourceLibraryPath.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceLibraryPath = value;
                OnPropertyChanged(nameof(SourceLibraryPath));
            }
        }


        private string _SourceLibraryPathDistance = string.Empty;
        public string SourceLibraryPathDistance
        {
            get { return _SourceLibraryPathDistance; }
            set
            {
                if (_SourceLibraryPathDistance == value) { return; }

                _SourceLibraryPathDistance = value;
                OnPropertyChanged(nameof(SourceLibraryPathDistance));
            }
        }


        private string _SourceCategory = string.Empty;
        public string SourceCategory
        {
            get { return _SourceCategory; }
            set
            {
                if (_SourceCategory != null && _SourceCategory.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceCategory = value;
                OnPropertyChanged(nameof(SourceCategory));
            }
        }

        private string _SourceCategoryDistance = string.Empty;
        public string SourceCategoryDistance
        {
            get { return _SourceCategoryDistance; }
            set
            {
                if (_SourceCategoryDistance == value) { return; }

                _SourceCategoryDistance = value;
                OnPropertyChanged(nameof(SourceCategoryDistance));
            }
        }


        private string _SourceOmniClass = string.Empty;
        public string SourceOmniClass
        {
            get { return _SourceOmniClass; }
            set
            {
                if (_SourceOmniClass != null && _SourceOmniClass.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceOmniClass = value;
                OnPropertyChanged(nameof(SourceOmniClass));
            }
        }


        private string _SourceOmniClassDistance = string.Empty;
        public string SourceOmniClassDistance
        {
            get { return _SourceOmniClassDistance; }
            set
            {
                if (_SourceOmniClassDistance == value) { return; }

                _SourceOmniClassDistance = value;
                OnPropertyChanged(nameof(SourceOmniClassDistance));
            }
        }


        private string _SourceUpdated = string.Empty;
        public string SourceUpdated
        {
            get { return _SourceUpdated; }
            set
            {
                if (_SourceUpdated != null && _SourceUpdated.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceUpdated = value;
                OnPropertyChanged(nameof(SourceUpdated));
            }
        }


        private string _SourceUpdatedDistance = string.Empty;
        public string SourceUpdatedDistance
        {
            get { return _SourceUpdatedDistance; }
            set
            {
                if (_SourceUpdatedDistance == value) { return; }

                _SourceUpdatedDistance = value;
                OnPropertyChanged(nameof(SourceUpdatedDistance));
            }
        }


        private string _SourceProduct = string.Empty;
        public string SourceProduct
        {
            get { return _SourceProduct; }
            set
            {
                if (_SourceProduct != null && _SourceProduct.Equals(value, StringComparison.CurrentCulture)) { return; }

                _SourceProduct = value;
                OnPropertyChanged(nameof(SourceProduct));
            }
        }


        private string _SourceProductDistance = string.Empty;
        public string SourceProductDistance
        {
            get { return _SourceProductDistance; }
            set
            {
                if (_SourceProductDistance == value) { return; }

                _SourceProductDistance = value;
                OnPropertyChanged(nameof(SourceProductDistance));
            }
        }


        private FamilyType _SourceSelectedFamilyType;
        public FamilyType SourceSelectedFamilyType
        {
            get { return _SourceSelectedFamilyType; }
            set
            {
                if (_SourceSelectedFamilyType != null
                    && _SourceSelectedFamilyType.Equals(value)) { return; }

                _SourceSelectedFamilyType = value;
                OnPropertyChanged(nameof(SourceSelectedFamilyType));

                UpdateSourceFamilyTypeParameters();
            }
        }

        private void UpdateSourceFamilyTypeParameters()
        {
            if (SourceSelectedFamilyType is null) { return; }

            var comparer = FamilyTypeParametersDublicateComparer.ParameterComparer;
            SourceFamilyTypeParameters.Clear();
            foreach (var parameter in SourceSelectedFamilyType.Parameters)
            {
                var viewModel = new ParameterViewModel { Parameter = parameter };
                SourceFamilyTypeParameters.Add(viewModel);

                if (OriginalSelectedFamilyType is null) { continue; }

                var originalParameter = OriginalSelectedFamilyType.ByName(parameter.Name);
                UpdateParameterDistance(viewModel, originalParameter, comparer);
            }
        }

        private void UpdateSourceFamilyTypeParameterDistances()
        {
            if (OriginalSelectedFamilyType is null) { return; }

            var comparer = FamilyTypeParametersDublicateComparer.ParameterComparer;
            foreach (var viewModel in SourceFamilyTypeParameters)
            {
                var originalParameter = OriginalSelectedFamilyType.ByName(viewModel.Parameter.Name);
                UpdateParameterDistance(viewModel, originalParameter, comparer);
            }
        }

        private static void UpdateParameterDistance(ParameterViewModel viewModel, Parameter parameter, IModelDuplicateComparer<Parameter> comparer)
        {
            if (parameter is null) { return; }

            viewModel.UpdateDistance(parameter, comparer);
        }

        public ObservableCollection<ParameterViewModel> SourceFamilyParameters { get; }
            = new ObservableCollection<ParameterViewModel>();

        public ObservableCollection<FamilyType> SourceFamilyTypes { get; }
            = new ObservableCollection<FamilyType>();

        public ObservableCollection<ParameterViewModel> SourceFamilyTypeParameters { get; }
            = new ObservableCollection<ParameterViewModel>();

        #endregion

        private string GetDistance(string propertyName)
        {
            if (FamilyComparer.HasByName(propertyName, out var comparer) == false)
            {
                return string.Empty;
            }

            return comparer.LevenstheinDistanceAsString(Original, Source);
        }


        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
