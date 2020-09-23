using DataSource.Metadata;
using RevitJournal.Library;
using RevitJournal.Revit.Filtering;
using RevitJournal.Revit.Filtering.Rules;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    public class RevitFamilyFilterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly RevitFilterManager manager = RevitFilterManager.Instance;

        public RevitFamilyFilterViewModel()
        {
            UpdateMetadataStatus();
            UpdateRevitApps();
            UpdateCategories();
            UpdateOmniClasses();
            UpdateFamilyBasis();
            UpdateFamilyParameters();
        }

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

        public string ValidName
        {
            get { return isValid.Name; }
        }

        private MetadataStatusValue isValid;
        public bool IsValid
        {
            get { return isValid.IsChecked; }
            set
            {
                if (isValid.IsChecked == value) { return; }

                isValid.IsChecked = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public string RepairableName
        {
            get { return isRepairable.Name; }
        }

        private MetadataStatusValue isRepairable;
        public bool IsRepairable
        {
            get { return isRepairable.IsChecked; }
            set
            {
                if (isRepairable.IsChecked == value) { return; }

                isRepairable.IsChecked = value;
                OnPropertyChanged(nameof(IsRepairable));
            }
        }

        public string ErrorName
        {
            get { return isError.Name; }
        }

        private MetadataStatusValue isError;
        public bool IsError
        {
            get { return isError.IsChecked; }
            set
            {
                if (isError.IsChecked == value) { return; }

                isError.IsChecked = value;
                OnPropertyChanged(nameof(IsError));
            }
        }

        internal void UpdateMetadataStatus()
        {
            foreach (var status in manager.GetValues(MetadataStatusRule.RuleKey))
            {
                if(!(status is MetadataStatusValue statusValue)) { continue; }

                switch (statusValue.Status)
                {
                    case MetadataStatus.Valid:
                        isValid = statusValue;
                        break;
                    case MetadataStatus.Repairable:
                        isRepairable = statusValue;
                        break;
                    case MetadataStatus.Error:
                        isError = statusValue;
                        break;
                    case MetadataStatus.Initial:
                        break;
                }
            }
        }

        public string ProductRuleName { get; set; } = string.Empty;
        public ObservableCollection<CheckedFilterViewModel> Products { get; }
            = new ObservableCollection<CheckedFilterViewModel>();

        public void UpdateRevitApps()
        {
            if(manager.HasRule(ProductRule.RuleKey, out var rule) == false) { return; }

            Products.Clear();
            ProductRuleName = rule.Name;
            foreach (var product in rule.Values)
            {
                var model = new CheckedFilterViewModel { FilterValue = product };
                Products.Add(model);
            }
        }

        public string ComponentRuleName { get; set; } = string.Empty;
        public ObservableCollection<CheckedFilterViewModel> BasicComponents { get; }
            = new ObservableCollection<CheckedFilterViewModel>();

        internal void UpdateFamilyBasis()
        {
            if(manager.HasRule(BasicComponentRule.RuleKey, out var rule) == false) { return; }

            BasicComponents.Clear();
            ComponentRuleName = rule.Name;
            foreach (var basis in rule.Values)
            {
                var model = new CheckedFilterViewModel { FilterValue = basis };
                BasicComponents.Add(model);
            }
        }

        public string CategoryRuleName { get; set; } = string.Empty;
        public ObservableCollection<CheckedFilterViewModel> Categories { get; }
            = new ObservableCollection<CheckedFilterViewModel>();

        internal void UpdateCategories()
        {
            if(manager.HasRule(CategoryRule.RuleKey, out var rule) == false) { return; }

            Categories.Clear();
            CategoryRuleName = rule.Name;
            foreach (var category in rule.Values)
            {
                var model = new CheckedFilterViewModel { FilterValue = category };
                Categories.Add(model);
            }
        }

        public string OmniClassRuleName { get; set; } = string.Empty;
        public ObservableCollection<CheckedFilterViewModel> OmniClasses { get; }
            = new ObservableCollection<CheckedFilterViewModel>();

        internal void UpdateOmniClasses()
        {
            if(manager.HasRule(OmniClassRule.RuleKey, out var rule) == false) { return; }

            OmniClasses.Clear();
            OmniClassRuleName = rule.Name;
            foreach (var omniClass in rule.Values)
            {
                var model = new CheckedFilterViewModel { FilterValue = omniClass };
                OmniClasses.Add(model);
            }
        }

        public string ParameterRuleName { get; set; } = string.Empty;
        public ObservableCollection<CheckedFilterViewModel> Parameters { get; }
            = new ObservableCollection<CheckedFilterViewModel>();

        internal void UpdateFamilyParameters()
        {
            if(manager.HasRule(ParameterRule.RuleKey, out var rule) == false) { return; }

            Parameters.Clear();
            ParameterRuleName = rule.Name;
            foreach (var parameter in rule.Values)
            {
                var model = new CheckedFilterViewModel { FilterValue = parameter };
                Parameters.Add(model);
            }
        }

        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
