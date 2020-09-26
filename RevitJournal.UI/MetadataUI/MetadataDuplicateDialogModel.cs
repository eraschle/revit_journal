using DataSource.Model.Family;
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
using System.Linq;
using System.Windows.Input;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.MetadataUI
{
    public class MetadataDuplicateDialogModel : ANotifyPropertyChangedModel
    {
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

        private bool isSearchOptionExpanded = true;
        public bool IsSearchOptionExpanded
        {
            get { return isSearchOptionExpanded; }
            set
            {
                if (isSearchOptionExpanded == value) { return; }

                isSearchOptionExpanded = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        public ObservableCollection<OriginalFamilyViewModel> Models { get; }
            = new ObservableCollection<OriginalFamilyViewModel>();


        private OriginalFamilyViewModel selectedModel;
        public OriginalFamilyViewModel SelectedModel
        {
            get { return selectedModel; }
            set
            {
                if (selectedModel != null && selectedModel.Equals(value)) { return; }

                selectedModel = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<DuplicateFamilyViewModel> Duplicated { get; }
            = new ObservableCollection<DuplicateFamilyViewModel>();


        #region Original Metadata

        private Family original;
        public Family Original
        {
            get { return original; }
            set
            {
                if (original != null && original.Equals(value)) { return; }

                original = value;
                NotifyPropertyChanged();
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


        private string originalName = string.Empty;
        public string OriginalName
        {
            get { return originalName; }
            set
            {
                if (originalName != null && StringUtils.Equals(originalName, value)) { return; }

                originalName = value;
                NotifyPropertyChanged();
            }
        }

        private string originalDisplayName = string.Empty;
        public string OriginalDisplayName
        {
            get { return originalDisplayName; }
            set
            {
                if (originalDisplayName != null && StringUtils.Equals(originalDisplayName, value)) { return; }

                originalDisplayName = value;
                NotifyPropertyChanged();
            }
        }

        private string originalLibraryPath = string.Empty;
        public string OriginalLibraryPath
        {
            get { return originalLibraryPath; }
            set
            {
                if (originalLibraryPath != null && StringUtils.Equals(originalLibraryPath, value)) { return; }

                originalLibraryPath = value;
                NotifyPropertyChanged();
            }
        }

        private string originalCategory = string.Empty;
        public string OriginalCategory
        {
            get { return originalCategory; }
            set
            {
                if (originalCategory != null && StringUtils.Equals(originalCategory, value)) { return; }

                originalCategory = value;
                NotifyPropertyChanged();
            }
        }

        private string originalOmniClass = string.Empty;
        public string OriginalOmniClass
        {
            get { return originalOmniClass; }
            set
            {
                if (originalOmniClass != null && StringUtils.Equals(originalOmniClass, value)) { return; }

                originalOmniClass = value;
                NotifyPropertyChanged();
            }
        }

        private string originalUpdated = string.Empty;
        public string OriginalUpdated
        {
            get { return originalUpdated; }
            set
            {
                if (originalUpdated != null && StringUtils.Equals(originalUpdated, value)) { return; }

                originalUpdated = value;
            }
        }

        private string originalProduct = string.Empty;
        public string OriginalProduct
        {
            get { return originalProduct; }
            set
            {
                if (originalProduct != null && StringUtils.Equals(originalProduct, value)) { return; }

                originalProduct = value;
                NotifyPropertyChanged();
            }
        }

        private FamilyType originalSelectedFamilyType;
        public FamilyType OriginalSelectedFamilyType
        {
            get { return originalSelectedFamilyType; }
            set
            {
                if (value is null ||
                    originalSelectedFamilyType != null &&
                    originalSelectedFamilyType.Equals(value)) { return; }

                originalSelectedFamilyType = value;
                NotifyPropertyChanged();
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

        private Family source;
        public Family Source
        {
            get { return source; }
            set
            {
                if (source != null && source.Equals(value)) { return; }

                source = value;
                NotifyPropertyChanged();
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

        private string sourceLevenstein = string.Empty;
        public string SourceLevenstein
        {
            get { return sourceLevenstein; }
            set
            {
                if (sourceLevenstein != null && StringUtils.Equals(sourceLevenstein, value)) { return; }

                sourceLevenstein = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceName = string.Empty;
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                if (sourceName != null && StringUtils.Equals(sourceName, value)) { return; }

                sourceName = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceNameDistance = string.Empty;
        public string SourceNameDistance
        {
            get { return sourceNameDistance; }
            set
            {
                if (sourceNameDistance == value) { return; }

                sourceNameDistance = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceDisplayName = string.Empty;
        public string SourceDisplayName
        {
            get { return sourceDisplayName; }
            set
            {
                if (sourceDisplayName != null && StringUtils.Equals(sourceDisplayName, value)) { return; }

                sourceDisplayName = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceDisplayNameDistance = string.Empty;
        public string SourceDisplayNameDistance
        {
            get { return sourceDisplayNameDistance; }
            set
            {
                if (sourceDisplayNameDistance == value) { return; }

                sourceDisplayNameDistance = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceLibraryPath = string.Empty;
        public string SourceLibraryPath
        {
            get { return sourceLibraryPath; }
            set
            {
                if (sourceLibraryPath != null && StringUtils.Equals(sourceLibraryPath, value)) { return; }

                sourceLibraryPath = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceLibraryPathDistance = string.Empty;
        public string SourceLibraryPathDistance
        {
            get { return sourceLibraryPathDistance; }
            set
            {
                if (sourceLibraryPathDistance == value) { return; }

                sourceLibraryPathDistance = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceCategory = string.Empty;
        public string SourceCategory
        {
            get { return sourceCategory; }
            set
            {
                if (sourceCategory != null && StringUtils.Equals(sourceCategory, value)) { return; }

                sourceCategory = value;
                NotifyPropertyChanged();
            }
        }

        private string sourceCategoryDistance = string.Empty;
        public string SourceCategoryDistance
        {
            get { return sourceCategoryDistance; }
            set
            {
                if (sourceCategoryDistance == value) { return; }

                sourceCategoryDistance = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceOmniClass = string.Empty;
        public string SourceOmniClass
        {
            get { return sourceOmniClass; }
            set
            {
                if (sourceOmniClass != null && StringUtils.Equals(sourceOmniClass, value)) { return; }

                sourceOmniClass = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceOmniClassDistance = string.Empty;
        public string SourceOmniClassDistance
        {
            get { return sourceOmniClassDistance; }
            set
            {
                if (sourceOmniClassDistance == value) { return; }

                sourceOmniClassDistance = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceUpdated = string.Empty;
        public string SourceUpdated
        {
            get { return sourceUpdated; }
            set
            {
                if (sourceUpdated != null && StringUtils.Equals(sourceUpdated, value)) { return; }

                sourceUpdated = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceUpdatedDistance = string.Empty;
        public string SourceUpdatedDistance
        {
            get { return sourceUpdatedDistance; }
            set
            {
                if (sourceUpdatedDistance == value) { return; }

                sourceUpdatedDistance = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceProduct = string.Empty;
        public string SourceProduct
        {
            get { return sourceProduct; }
            set
            {
                if (sourceProduct != null && StringUtils.Equals(sourceProduct, value)) { return; }

                sourceProduct = value;
                NotifyPropertyChanged();
            }
        }


        private string sourceProductDistance = string.Empty;
        public string SourceProductDistance
        {
            get { return sourceProductDistance; }
            set
            {
                if (sourceProductDistance == value) { return; }

                sourceProductDistance = value;
                NotifyPropertyChanged();
            }
        }


        private FamilyType sourceSelectedFamilyType;
        public FamilyType SourceSelectedFamilyType
        {
            get { return sourceSelectedFamilyType; }
            set
            {
                if (sourceSelectedFamilyType != null
                    && sourceSelectedFamilyType.Equals(value)) { return; }

                sourceSelectedFamilyType = value;
                NotifyPropertyChanged();

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
    }
}
