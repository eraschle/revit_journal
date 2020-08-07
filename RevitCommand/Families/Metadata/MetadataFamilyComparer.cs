using DataSource.Model.Catalog;
using DataSource.Model.Family;
using DataSource.Model.ProductData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCommand.Families.Metadata
{
    public class MetadataFamilyComparer : IEqualityComparer<Family>
    {
        public bool Equals(Family family, Family other)
        {
            return family != null && other != null
                && HasChangedCategory(family, other, out _) == false
                && HasChangedOmniClass(family, other, out _) == false;
        }

        public int GetHashCode(Family obj)
        {
            var hashCode = -992239906;
            if (obj.HasCategory(out Category category))
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<Category>.Default.GetHashCode(category);
            }
            if (obj.HasOmniClass(out var omniClass))
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<OmniClass>.Default.GetHashCode(omniClass);
            }
            return hashCode;
        }

        public bool HasChangedCategory(Family family, Family revit, out Category category)
        {
            category = null;
            if (family.HasCategory(out var familyCategory)
                && revit.HasCategory(out var revitCategory)
                && familyCategory.Equals(revitCategory) == false)
            {
                category = revitCategory;
            }
            return category != null;
        }

        public bool HasChangedOmniClass(Family family, Family revit, out OmniClass omniClass)
        {
            omniClass = null;
            if (family.HasOmniClass(out var familyOmniClass)
                && revit.HasOmniClass(out var revitOmniClass)
                && familyOmniClass.Equals(revitOmniClass) == false)
            {
                omniClass = familyOmniClass;
            }
            return omniClass != null;
        }
    }
}
