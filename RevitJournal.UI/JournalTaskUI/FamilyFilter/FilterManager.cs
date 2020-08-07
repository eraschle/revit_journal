using DataSource.Metadata;
using DataSource.Model.Family;
using RevitJournalUI.JournalTaskUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.FamilyFilter
{
    public class FilterManager
    {
        private const string FileNotExist = "External file not exist";
        private const string Status = "Status";
        private const string Product = "App";
        private const string Category = "Category";
        private const string OmnicClass = "OmniClass";
        private const string Basis = "Basis";
        private const string Parameter = "Parameter";

        public bool CheckedMetadatFileNotExist { get; private set; } = false;
        public ICollection<MetadataStatus> CheckedMetadataStatus { get; private set; } = new List<MetadataStatus>();
        public ICollection<string> CheckedApps { get; private set; } = new List<string>();
        public ICollection<string> CheckedCategories { get; private set; } = new List<string>();
        public ICollection<string> CheckedOmniClasses { get; private set; } = new List<string>();
        public ICollection<string> CheckedFamilyBasis { get; private set; } = new List<string>();
        public ICollection<string> CheckedFamilyParameters { get; private set; } = new List<string>();

        public bool MetadatFileNotExist { get; private set; } = false;
        public ICollection<MetadataStatus> MetadataStatus { get; private set; } = new List<MetadataStatus>();
        public ICollection<string> Products { get; private set; } = new List<string>();
        public ICollection<string> Categories { get; private set; } = new List<string>();
        public ICollection<string> OmniClasses { get; private set; } = new List<string>();
        public ICollection<string> FamilyBasis { get; private set; } = new List<string>();
        public ICollection<string> FamilyParameters { get; private set; } = new List<string>();

        public void UpdateFilter(RevitFamilyFilterViewModel model)
        {
            if (model is null) { throw new ArgumentNullException(nameof(model)); }

            MetadatFileNotExist = model.MetadataFileNotExist;
            CheckedMetadatFileNotExist = MetadatFileNotExist;

            MetadataStatus = model.CheckedMetadataStatus;
            CheckedMetadataStatus = new List<MetadataStatus>(MetadataStatus);

            Products = model.CheckedRevitApps;
            CheckedApps = new List<string>(Products);

            Categories = model.CheckedCategories;
            CheckedCategories = new List<string>(Categories);

            OmniClasses = model.CheckedOmniClasses;
            CheckedOmniClasses = new List<string>(OmniClasses);

            FamilyBasis = model.CheckedFamilyBasis;
            CheckedFamilyBasis = new List<string>(FamilyBasis);

            FamilyParameters = model.CheckedFamilyParameters;
            CheckedFamilyParameters = new List<string>(FamilyParameters);
        }

        public void ClearFilter()
        {
            MetadatFileNotExist = false;
            MetadataStatus.Clear();
            Products.Clear();
            Categories.Clear();
            OmniClasses.Clear();
            FamilyBasis.Clear();
            FamilyParameters.Clear();
        }

        private bool NoFilterConfigured()
        {
            return MetadatFileNotExist == false
                && MetadataStatus.Count == 0
                && Products.Count == 0
                && Categories.Count == 0
                && OmniClasses.Count == 0
                && FamilyBasis.Count == 0
                && FamilyParameters.Count == 0;
        }

        public bool FileFilter(FamilyViewModel model)
        {
            if (model is null) { throw new ArgumentNullException(nameof(model)); }

            if (NoFilterConfigured()) { return true; }

            if (MetadatFileNotExist && model.RevitFamily.HasFileMetadata == false)
            {
                return true;
            }

            var filtered = true;
            if (MetadataStatus.Count > 0)
            {
                var metadataStatus = model.RevitFamily.MetadataStatus;
                filtered &= MetadataStatus.Contains(metadataStatus);
            }

            if (model.RevitFamily.HasValidMetadata == false) { return filtered; }

            var metadata = model.RevitFamily.Metadata;
            if (filtered && Products.Count > 0
                && metadata.HasProduct(out var product))
            {
                filtered &= Products.Contains(product.ProductName);
            }

            if (filtered && Categories.Count > 0
                && metadata.HasCategory(out var category))
            {
                filtered &= Categories.Contains(category.Name);
            }

            if (filtered && FamilyBasis.Count > 0
                && metadata.HasByName(Family.BasicComponent, out Parameter basicComponent))
            {
                filtered &= FamilyBasis.Contains(basicComponent.Value);
            }

            if (filtered && OmniClasses.Count > 0
                && metadata.HasOmniClass(out var omniClass))
            {
                filtered &= OmniClasses.Contains(omniClass.NumberAndName);
            }

            if (filtered)
            {
                foreach (var parametere in FamilyParameters)
                {
                    filtered &= metadata.HasByName(parametere, out Parameter metaParameter);
                    if (filtered == false) { break; }
                }
            }
            return filtered;
        }

        public bool DirectoryFilter(DirectoryViewModel model)
        {
            if (model is null) { return true; }

            foreach (var directory in model.SubDirectories)
            {
                DirectoryFilter(directory);
            }
            var revitFamilies = model.RecursiveFamilyViewModel;
            var visibility = Visibility.Collapsed;
            if (revitFamilies.Any(file => FileFilter(file)))
            {
                visibility = Visibility.Visible;
            }
            model.Visibility = visibility;

            return true;
        }

        public IEnumerable<FilterViewModel> CheckedFilterViewModels()
        {
            var viewModels = new List<FilterViewModel>();
            if (MetadatFileNotExist)
            {
                viewModels.Add(new FilterViewModel { Group = Status, Filter = FileNotExist });
            }
            foreach (var status in MetadataStatus)
            {
                viewModels.Add(new FilterViewModel { Group = Status, Filter = Metadata.GetStatusName(status) });
            }
            foreach (var product in Products)
            {
                viewModels.Add(new FilterViewModel { Group = Product, Filter = product });
            }
            foreach (var category in Categories)
            {
                viewModels.Add(new FilterViewModel { Group = Category, Filter = category });
            }
            foreach (var omniClass in OmniClasses)
            {
                viewModels.Add(new FilterViewModel { Group = OmnicClass, Filter = omniClass });
            }
            foreach (var basis in FamilyBasis)
            {
                viewModels.Add(new FilterViewModel { Group = Basis, Filter = basis });
            }
            foreach (var parameter in FamilyParameters)
            {
                viewModels.Add(new FilterViewModel { Group = Parameter, Filter = parameter });
            }
            return viewModels;
        }
    }
}
