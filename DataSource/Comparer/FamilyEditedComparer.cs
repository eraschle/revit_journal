using DataSource.Model.Metadata;
using DataSource.Models.Catalog;
using System.Collections.Generic;

namespace DataSource.Comparer
{
    public class FamilyEditedComparer : IEqualityComparer<Family>
    {
        private readonly ParameterEditedComparer parameters = new ParameterEditedComparer();
        private readonly FamilyTypeEditedComparer familyTypes = new FamilyTypeEditedComparer();

        public bool Equals(Family family, Family other)
        {
            return family != null && other != null
                && family.Name == other.Name
                && family.LibraryPath == other.LibraryPath
                && family.Category == other.Category
                && family.OmniClass == other.OmniClass
                && parameters.ParametersEquals(family.Parameters, other.Parameters)
                && familyTypes.FamilyTypesEquals(family.FamilyTypes, other.FamilyTypes);
        }

        public int GetHashCode(Family obj)
        {
            var hashCode = 985813280;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.LibraryPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<Category>.Default.GetHashCode(obj.Category);
            hashCode = hashCode * -1521134295 + EqualityComparer<OmniClass>.Default.GetHashCode(obj.OmniClass);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<Parameter>>.Default.GetHashCode(obj.Parameters);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<FamilyType>>.Default.GetHashCode(obj.FamilyTypes);
            return hashCode;
        }
    }
}
