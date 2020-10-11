using DataSource.Model.Metadata;
using System.Collections.Generic;

namespace DataSource.Comparer
{
    public class FamilyTypeEditedComparer : IEqualityComparer<FamilyType>
    {
        private readonly ParameterEditedComparer parameters = new ParameterEditedComparer();

        public bool FamilyTypesEquals(IList<FamilyType> familyTypes, IList<FamilyType> other)
        {
            var equals = familyTypes != null && other != null 
                           && familyTypes.Count == other.Count;
            if (equals == false) { return false; }

            foreach (var familyType in familyTypes)
            {
                equals &= other.Contains(familyType);
                if (equals == false) { break; }

                var idx = other.IndexOf(familyType);
                equals &= Equals(familyType, other[idx]);
                if (equals == false) { break; }
            }
            return equals;
        }

        public bool Equals(FamilyType familyType, FamilyType other)
        {
            var familyTypeEquals = familyType != null && other != null
                && familyType.Name == other.Name
                && parameters.ParametersEquals(familyType.Parameters, other.Parameters);
            return familyTypeEquals;
        }

        public int GetHashCode(FamilyType obj)
        {
            var hashCode = -691830078;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<Parameter>>.Default.GetHashCode(obj.Parameters);
            return hashCode;
        }
    }
}
