using DataSource.Model.Metadata;
using RevitJournal.Duplicate.Comparer.FamilyTypeComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Duplicate.Comparer
{
    public class FamilyTypeDuplicateComparer : ILevenstheinComparer<FamilyType>, IModelDuplicateComparer<FamilyType>
    {
        public static ICollection<IDuplicateComparer<FamilyType>> AllComparers()
        {
            return new List<IDuplicateComparer<FamilyType>>
            {
                new FamilyTypeNameDublicateComparer(false),
                new FamilyTypeParametersDublicateComparer(true)
            };
        }

        public FamilyTypeDuplicateComparer() : this(AllComparers()) { }

        public FamilyTypeDuplicateComparer(ICollection<IDuplicateComparer<FamilyType>> comparers)
        {
            PropertyComparers = comparers;
        }

        public ICollection<IDuplicateComparer<FamilyType>> PropertyComparers { get; set; }

        public ICollection<IDuplicateComparer<FamilyType>> UsedComparers
        {
            get { return PropertyComparers.Where(cmp => cmp.UseComparer).ToList(); }
        }

        public bool HasByName(string propertyName, out IDuplicateComparer<FamilyType> comparer)
        {
            comparer = ByName(propertyName);
            return comparer != null;
        }

        public IDuplicateComparer<FamilyType> ByName(string propertyName)
        {
            return PropertyComparers.FirstOrDefault(cmp => cmp.PropertyName.Equals(propertyName, StringComparison.CurrentCulture));
        }

        public int Compare(FamilyType familyType, FamilyType other)
        {
            return LevenstheinDistance(familyType, other);
        }

        public bool Equals(FamilyType familyType, FamilyType other)
        {
            var dublicateEquals = UsedComparers.Select(cmp => cmp.Equals(familyType, other));
            return familyType != null && other != null &&
                dublicateEquals.All(result => result == true);
        }

        public int GetHashCode(FamilyType obj)
        {
            var hashCode = 673411578;
            foreach (var comparer in UsedComparers)
            {
                hashCode = hashCode * -1521134295 + comparer.GetHashCode(obj);
            }
            return hashCode;
        }

        public int LevenstheinDistance(FamilyType model, FamilyType other)
        {
            var distance = 0;
            foreach (var comparer in PropertyComparers)
            {
                distance += comparer.LevenstheinDistance(model, other);
            }
            return distance;
        }

        public string LevenstheinDistanceAsString(FamilyType model, FamilyType other)
        {
            var distance = LevenstheinDistance(model, other);
            return LevenstheinHelper.LevenstheinAsString(distance);
        }
    }
}
