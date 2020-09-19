using DataSource.Metadata;
using RevitJournal.Library;
using System;
using System.Collections.Generic;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    public class FilterManagerViewModel
    {
        private const string FileNotExist = "External file not exist";
        private const string Status = "Status";
        private const string Product = "App";
        private const string Category = "Category";
        private const string OmnicClass = "OmniClass";
        private const string Basis = "Basis";
        private const string Parameter = "Parameter";

        internal FilterManager Manager { get; set; }

        public bool CheckedMetadatFileNotExist { get; private set; } = false;
        public ICollection<MetadataStatus> CheckedMetadataStatus { get; private set; } = new List<MetadataStatus>();
        public ICollection<string> CheckedApps { get; private set; } = new List<string>();
        public ICollection<string> CheckedCategories { get; private set; } = new List<string>();
        public ICollection<string> CheckedOmniClasses { get; private set; } = new List<string>();
        public ICollection<string> CheckedFamilyBasis { get; private set; } = new List<string>();
        public ICollection<string> CheckedFamilyParameters { get; private set; } = new List<string>();

        public void UpdateFilter(RevitFamilyFilterViewModel model)
        {
            if (model is null) { throw new ArgumentNullException(nameof(model)); }

            Manager.MetadatFileNotExist = model.MetadataFileNotExist;
            CheckedMetadatFileNotExist = Manager.MetadatFileNotExist;

            Manager.MetadataStatus.IntersectWith(model.CheckedMetadataStatus);
            CheckedMetadataStatus = new List<MetadataStatus>(Manager.MetadataStatus);

            Manager.Products.IntersectWith(model.CheckedRevitApps);
            CheckedApps = new List<string>(Manager.Products);

            Manager.Categories.IntersectWith(model.CheckedCategories);
            CheckedCategories = new List<string>(Manager.Categories);

            Manager.OmniClasses.IntersectWith(model.CheckedOmniClasses);
            CheckedOmniClasses = new List<string>(Manager.OmniClasses);

            Manager.FamilyBasis.IntersectWith(model.CheckedFamilyBasis);
            CheckedFamilyBasis = new List<string>(Manager.FamilyBasis);

            Manager.FamilyParameters.IntersectWith(model.CheckedFamilyParameters);
            CheckedFamilyParameters = new List<string>(Manager.FamilyParameters);
        }

        public void ClearFilter()
        {
            Manager.ClearFilter();
        }

        public IEnumerable<FilterViewModel> CheckedFilterViewModels()
        {
            var viewModels = new List<FilterViewModel>();
            if (Manager.MetadatFileNotExist)
            {
                viewModels.Add(new FilterViewModel { Group = Status, Filter = FileNotExist });
            }
            foreach (var status in Manager.MetadataStatus)
            {
                viewModels.Add(new FilterViewModel { Group = Status, Filter = Metadata.GetStatusName(status) });
            }
            foreach (var product in Manager.Products)
            {
                viewModels.Add(new FilterViewModel { Group = Product, Filter = product });
            }
            foreach (var category in Manager.Categories)
            {
                viewModels.Add(new FilterViewModel { Group = Category, Filter = category });
            }
            foreach (var omniClass in Manager.OmniClasses)
            {
                viewModels.Add(new FilterViewModel { Group = OmnicClass, Filter = omniClass });
            }
            foreach (var basis in Manager.FamilyBasis)
            {
                viewModels.Add(new FilterViewModel { Group = Basis, Filter = basis });
            }
            foreach (var parameter in Manager.FamilyParameters)
            {
                viewModels.Add(new FilterViewModel { Group = Parameter, Filter = parameter });
            }
            return viewModels;
        }
    }
}
