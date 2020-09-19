using DataSource.Metadata;
using DataSource.Model.Family;
using RevitJournalUI.JournalTaskUI.Models;
using RevitJournalUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    public class RevitFamilyFilterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal const string DefaultNoValue = "No Value";
        private const string PrefixLoadingProgress = "Loading";

        private BackgroundWorker WorkerMetadata;

        private readonly SortedSet<string> SortedRevitApps = new SortedSet<string>();
        private readonly SortedSet<string> SortedCategories = new SortedSet<string>();
        private readonly SortedSet<string> SortedFamilyBasis = new SortedSet<string>();
        private readonly SortedSet<string> SortedOmniClasses = new SortedSet<string>();
        private readonly SortedSet<string> SortedFamilyParameters = new SortedSet<string>();

        private FilterManagerViewModel FilterManager;

        #region Filter

        private bool _MetadataFileNotExist = false;
        public bool MetadataFileNotExist
        {
            get { return _MetadataFileNotExist; }
            set
            {
                if (_MetadataFileNotExist == value) { return; }

                _MetadataFileNotExist = value;
                OnPropertyChanged(nameof(MetadataFileNotExist));
            }
        }

        private bool _IsValid = false;
        public bool IsValid
        {
            get { return _IsValid; }
            set
            {
                if (_IsValid == value) { return; }

                _IsValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private bool _IsRepairable = false;
        public bool IsRepairable
        {
            get { return _IsRepairable; }
            set
            {
                if (_IsRepairable == value) { return; }

                _IsRepairable = value;
                OnPropertyChanged(nameof(IsRepairable));
            }
        }

        private bool _IsError = false;
        public bool IsError
        {
            get { return _IsError; }
            set
            {
                if (_IsError == value) { return; }

                _IsError = value;
                OnPropertyChanged(nameof(IsError));
            }
        }

        public ICollection<MetadataStatus> CheckedMetadataStatus
        {
            get
            {
                var metadata = new List<MetadataStatus>();
                if (IsValid) { metadata.Add(MetadataStatus.Valid); }
                if (IsRepairable) { metadata.Add(MetadataStatus.Repairable); }
                if (IsError) { metadata.Add(MetadataStatus.Error); }
                return metadata;
            }
        }

        private void UpdateMetadataStatus()
        {
            var metadata = FilterManager.CheckedMetadataStatus;
            if(metadata.Count == 0) { return; }

            MetadataFileNotExist = FilterManager.CheckedMetadatFileNotExist;
            IsValid = metadata.Contains(MetadataStatus.Valid);
            IsRepairable = metadata.Contains(MetadataStatus.Repairable);
            IsError = metadata.Contains(MetadataStatus.Error);
        }

        public ObservableCollection<CheckedDisplayViewModel> RevitApps { get; }
            = new ObservableCollection<CheckedDisplayViewModel>();

        public void UpdateRevitApps(ICollection<string> revitApps)
        {
            if (revitApps is null || revitApps.Count == 0) { return; }

            RevitApps.Clear();
            foreach (var revitApp in revitApps)
            {
                var isChecked = FilterManager.CheckedApps.Contains(revitApp);
                var model = new CheckedDisplayViewModel { DisplayName = revitApp, Checked = isChecked };
                RevitApps.Add(model);
            }
        }

        public ICollection<string> CheckedRevitApps { get { return GetChecked(RevitApps); } }

        public ObservableCollection<CheckedDisplayViewModel> FamilyBasis { get; }
            = new ObservableCollection<CheckedDisplayViewModel>();

        public void UpdateFamilyBasis(ICollection<string> familyBasis)
        {
            if (familyBasis is null || familyBasis.Count == 0) { return; }

            FamilyBasis.Clear();
            foreach (var basis in familyBasis)
            {
                var isChecked = FilterManager.CheckedFamilyBasis.Contains(basis);
                var model = new CheckedDisplayViewModel { DisplayName = basis, Checked = isChecked };
                FamilyBasis.Add(model);
            }
        }

        public ICollection<string> CheckedFamilyBasis { get { return GetChecked(FamilyBasis); } }

        public ObservableCollection<CheckedDisplayViewModel> Categories { get; }
            = new ObservableCollection<CheckedDisplayViewModel>();

        public void UpdateCategories(ICollection<string> categories)
        {
            if (categories is null || categories.Count == 0) { return; }

            Categories.Clear();
            foreach (var category in categories)
            {
                var isChecked = FilterManager.CheckedCategories.Contains(category);
                var model = new CheckedDisplayViewModel { DisplayName = category, Checked = isChecked };
                Categories.Add(model);
            }
        }

        public ICollection<string> CheckedCategories { get { return GetChecked(Categories); } }

        public ObservableCollection<CheckedDisplayViewModel> OmniClasses { get; }
            = new ObservableCollection<CheckedDisplayViewModel>();

        public void UpdateOmniClasses(ICollection<string> omniClasses)
        {
            if (omniClasses is null || omniClasses.Count == 0) { return; }

            OmniClasses.Clear();
            foreach (var omniClass in omniClasses)
            {
                var isChecked = FilterManager.CheckedOmniClasses.Contains(omniClass);
                var model = new CheckedDisplayViewModel { DisplayName = omniClass, Checked = isChecked };
                OmniClasses.Add(model);
            }
        }

        public ICollection<string> CheckedOmniClasses { get { return GetChecked(OmniClasses); } }


        public ObservableCollection<CheckedDisplayViewModel> FamilyParameters { get; }
            = new ObservableCollection<CheckedDisplayViewModel>();

        public void UpdateFamilyParameters(ICollection<string> omniClasses)
        {
            if (omniClasses is null || omniClasses.Count == 0) { return; }

            FamilyParameters.Clear();
            foreach (var parameter in omniClasses)
            {
                var isChecked = FilterManager.CheckedFamilyParameters.Contains(parameter);
                var model = new CheckedDisplayViewModel { DisplayName = parameter, Checked = isChecked };
                FamilyParameters.Add(model);
            }
        }
        
        public ICollection<string> CheckedFamilyParameters { get { return GetChecked(FamilyParameters); } }

        private static ICollection<string> GetChecked(ObservableCollection<CheckedDisplayViewModel> models)
        {
            return models.Where(model => model.Checked)
                         .Select(model => model.DisplayName)
                         .ToList();
        }

        #endregion

        #region Loading Filter

        private Visibility _LoadedVisibility = Visibility.Hidden;
        public Visibility LoadedVisibility
        {
            get { return _LoadedVisibility; }
            set
            {
                if (_LoadedVisibility == value) { return; }

                _LoadedVisibility = value;
                OnPropertyChanged(nameof(LoadedVisibility));
            }
        }

        private Visibility _LoadingVisibility = Visibility.Visible;
        public Visibility LoadingVisibility
        {
            get { return _LoadingVisibility; }
            set
            {
                if (_LoadingVisibility == value) { return; }

                _LoadingVisibility = value;
                OnPropertyChanged(nameof(LoadingVisibility));
            }
        }

        private string _LoadingProgessText = PrefixLoadingProgress;
        public string LoadingProgessText
        {
            get { return _LoadingProgessText; }
            set
            {
                if (_LoadingProgessText.Equals(value, StringComparison.CurrentCulture)) { return; }

                _LoadingProgessText = value;
                OnPropertyChanged(nameof(LoadingProgessText));
            }
        }

        private int _LoadingProgess = 0;
        public int LoadingProgess
        {
            get { return _LoadingProgess; }
            set
            {
                if (_LoadingProgess == value) { return; }

                _LoadingProgess = value;
                OnPropertyChanged(nameof(LoadingProgess));
            }
        }

        #endregion

        #region Update Filters

        public void ClearFilterCollections()
        {
            SortedRevitApps.Clear();
            SortedCategories.Clear();
            SortedFamilyBasis.Clear();
            SortedOmniClasses.Clear();
            SortedFamilyParameters.Clear();
        }

        public void LoadFamilyMetadata(ObservableCollection<DirectoryViewModel> viewModels, FilterManagerViewModel configuredFilter)
        {
            LoadedVisibility = Visibility.Hidden;
            LoadingVisibility = Visibility.Visible;
            ClearFilterCollections();
            FilterManager = configuredFilter;

            WorkerMetadata = MetadataBackgroundWorker.CreateWorker();
            WorkerMetadata.ProgressChanged += new ProgressChangedEventHandler(OnProgressChangedMetadata);
            WorkerMetadata.ProgressChanged += new ProgressChangedEventHandler(OnProgressChangedVersions);
            WorkerMetadata.ProgressChanged += new ProgressChangedEventHandler(OnProgressChangedCategories);
            WorkerMetadata.ProgressChanged += new ProgressChangedEventHandler(OnProgressChangedComponents);
            WorkerMetadata.ProgressChanged += new ProgressChangedEventHandler(OnProgressChangedOmniClasses);
            WorkerMetadata.ProgressChanged += new ProgressChangedEventHandler(OnProgressChangedBoolParameters);
            WorkerMetadata.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnProgressChangedMetadataCompleted);

            WorkerMetadata.RunWorkerAsync(viewModels);
        }

        private void OnProgressChangedMetadata(object sender, ProgressChangedEventArgs args)
        {
            if (args is null) { return; }

            LoadingProgess = args.ProgressPercentage;
            LoadingProgessText = PrefixLoadingProgress + " [" + LoadingProgess + "%]";
        }

        private bool HasRevitMetadata(FamilyViewModel model, out Family family)
        {
            family = null;
            var container = model.FileHandler.File;
            if (container.HasValidMetadata)
            {
                family = container.Metadata;
            }
            return family != null;
        }

        public void OnProgressChangedVersions(object sender, ProgressChangedEventArgs args)
        {
            if (args is null || !(args.UserState is FamilyViewModel model)
                || HasRevitMetadata(model, out var metadata) == false
                || metadata.HasProduct(out var product) == false
                || SortedRevitApps.Contains(product.ProductName)) { return; }

            SortedRevitApps.Add(product.ProductName);
        }

        private void OnProgressChangedCategories(object sender, ProgressChangedEventArgs args)
        {
            if (args is null || !(args.UserState is FamilyViewModel model)
                || HasRevitMetadata(model, out var metadata) == false
                || metadata.HasCategory(out var category) == false
                || SortedCategories.Contains(category.Name)) { return; }

            SortedCategories.Add(category.Name);
        }

        private void OnProgressChangedComponents(object sender, ProgressChangedEventArgs args)
        {
            if (args is null || !(args.UserState is FamilyViewModel model)
                || HasRevitMetadata(model, out var metadata) == false
                || metadata.HasByName(Family.BasicComponent, out Parameter parameter) == false
                || SortedFamilyBasis.Contains(parameter.Value)) { return; }

            SortedFamilyBasis.Add(parameter.Value);
        }

        private void OnProgressChangedOmniClasses(object sender, ProgressChangedEventArgs args)
        {
            if (args is null || !(args.UserState is FamilyViewModel model)
                || HasRevitMetadata(model, out var metadata) == false) { return; }

            var omniClassValue = DefaultNoValue;
            if(metadata.HasOmniClass(out var omniClass))
            {
                omniClassValue = omniClass.NumberAndName;
            }

            if (SortedOmniClasses.Contains(omniClassValue)) { return; }

            SortedOmniClasses.Add(omniClassValue);
        }

        private void OnProgressChangedBoolParameters(object sender, ProgressChangedEventArgs args)
        {
            if (args is null
                || !(args.UserState is FamilyViewModel model)
                || HasRevitMetadata(model, out var metadata) == false) { return; }

            var booleanParameters = metadata.Parameters.Where(par => IsBoolType(par));
            foreach (var parameter in booleanParameters)
            {
                if (SortedFamilyParameters.Contains(parameter.Name)) { continue; }

                SortedFamilyParameters.Add(parameter.Name);
            }
        }

        private bool IsBoolType(Parameter parameter)
        {
            if (parameter is null) { return false; }

            var valueType = parameter.ValueType;
            return valueType != null && valueType.Equals(Parameter.BooleanValueType, StringComparison.CurrentCulture);
        }

        private void OnProgressChangedMetadataCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            UpdateMetadataStatus();
            UpdateRevitApps(SortedRevitApps);
            UpdateCategories(SortedCategories);
            UpdateOmniClasses(SortedOmniClasses);
            UpdateFamilyBasis(SortedFamilyBasis);
            UpdateFamilyParameters(SortedFamilyParameters);

            WorkerMetadata.Dispose();

            LoadingVisibility = Visibility.Collapsed;
            LoadedVisibility = Visibility.Visible;
        }

        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
