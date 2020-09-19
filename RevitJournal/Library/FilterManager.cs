using DataSource.Metadata;
using DataSource.Model.Catalog;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Library
{
    public class FilterManager
    {
        public bool MetadatFileNotExist { get; set; } = false;

        public HashSet<MetadataStatus> MetadataStatus { get; } = new HashSet<MetadataStatus>();

        public HashSet<string> Products { get; } = new HashSet<string>();

        public HashSet<string> Categories { get; } = new HashSet<string>();

        public HashSet<string> OmniClasses { get; } = new HashSet<string>();

        public HashSet<string> FamilyBasis { get; } = new HashSet<string>();

        public HashSet<string> FamilyParameters { get; } = new HashSet<string>();


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

        public bool NoFilter()
        {
            return MetadatFileNotExist == false
                && MetadataStatus.Count == 0
                && Products.Count == 0
                && Categories.Count == 0
                && OmniClasses.Count == 0
                && FamilyBasis.Count == 0
                && FamilyParameters.Count == 0;
        }

        public bool FileFilter(RevitFamily family)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }

            if (NoFilter()) { return true; }

            if (MetadatFileNotExist && family.HasFileMetadata == false)
            {
                return true;
            }

            var filtered = true;
            if (HasStatus())
            {
                filtered &= IsStatus(family);
            }

            if (family.HasValidMetadata == false) { return filtered; }

            var metadata = family.Metadata;
            if (filtered && HasProduct(family, out var product))
            {
                filtered &= IsProduct(product);
            }

            if (filtered && HasCategory(family, out var category))
            {
                filtered &= IsCategory(category);
            }

            if (filtered && HasFamilyBasis(family, out Parameter basicComponent))
            {
                filtered &= IsFamilyBasis(basicComponent);
            }

            if (filtered && HasOmniClass(family, out var omniClass))
            {
                filtered &= IsOmniClass(omniClass);
            }
            return filtered && HasParameters(family);
        }

        public bool HasProduct(RevitFamily family, out RevitApp product)
        {
            product = null;
            return Products.Count > 0 && family is object
                && family.Metadata.HasProduct(out product);
        }

        public bool IsProduct(RevitApp product)
        {
            return product is object && Products.Contains(product.ProductName);
        }

        public bool HasStatus()
        {
            return MetadataStatus.Count > 0;
        }

        public bool IsStatus(RevitFamily family)
        {
            return family is object && MetadataStatus.Contains(family.MetadataStatus);
        }

        public bool IsDataNotExist(RevitFamily family)
        {
            return MetadatFileNotExist && family is object && family.HasFileMetadata == false;
        }

        public bool HasCategory(RevitFamily family, out Category category)
        {
            category = null;
            return Categories.Count > 0 && family is object
                && family.Metadata.HasCategory(out category);
        }

        public bool IsCategory(Category category)
        {
            return category is object && Categories.Contains(category.Name);
        }

        public bool DirectoryFilter(RevitDirectory directory)
        {
            if (directory is null) { return true; }

            foreach (var folder in directory.Subfolder)
            {
                DirectoryFilter(folder);
            }
            return true;
        }

        public bool IsFamilyBasis(Parameter basicComponent)
        {
            return basicComponent is object && FamilyBasis.Contains(basicComponent.Value);
        }

        public bool HasFamilyBasis(RevitFamily family, out Parameter basicComponent)
        {
            basicComponent = null;
            return FamilyBasis.Count > 0 && family is object
                && family.Metadata.HasByName(Family.BasicComponent, out basicComponent);
        }

        public bool HasOmniClass(RevitFamily family, out OmniClass omniClass)
        {
            omniClass = null;
            return OmniClasses.Count > 0 && family is object
                && family.Metadata.HasOmniClass(out omniClass);
        }

        public bool IsOmniClass(OmniClass omniClass)
        {
            return omniClass is object && OmniClasses.Contains(omniClass.NumberAndName);
        }

        public bool HasParameters(RevitFamily family)
        {
            return family is object && FamilyParameters.All(par => family.Metadata.HasByName(par, out Parameter _));
        }
    }
}
