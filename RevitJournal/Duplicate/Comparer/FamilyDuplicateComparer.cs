using DataSource.Model.Metadata;
using RevitJournal.Duplicate.Comparer.FamilyComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Duplicate.Comparer
{
    public class FamilyDuplicateComparer : ILevenstheinComparer<Family>, IModelDuplicateComparer<Family>
    {
        private static ICollection<IDuplicateComparer<Family>> AllComparers()
        {
            return new List<IDuplicateComparer<Family>>
            {
                new FamilyNameDublicateComparer(false),
                new FamilyDisplayNameDublicateComparer(false),
                new FamilyLibraryPathDublicateComparer(false),
                new FamilyCategoryDublicateComparer(false),
                new FamilyOmniClassDublicateComparer(false),
                new FamilyUpdatedDublicateComparer(false),
                new FamilyProductDublicateComparer(false),
                new FamilyFamilyTypesDublicateComparer(true),
                new FamilyParametersDublicateComparer(true)
            };
        }

        public FamilyDuplicateComparer() : this(AllComparers()) { }

        public FamilyDuplicateComparer(ICollection<IDuplicateComparer<Family>> comparers)
        {
            PropertyComparers = comparers;
        }

        public ICollection<IDuplicateComparer<Family>> PropertyComparers { get; set; }

        public ICollection<IDuplicateComparer<Family>> UsedComparers
        {
            get { return PropertyComparers.Where(cmp => cmp.UseComparer).ToList(); }
        }

        public bool HasByName(string propertyName, out IDuplicateComparer<Family> comparer)
        {
            comparer = ByName(propertyName);
            return comparer != null;
        }

        public IDuplicateComparer<Family> ByName(string propertyName)
        {
            return PropertyComparers.FirstOrDefault(cmp => cmp.PropertyName.Equals(propertyName, StringComparison.CurrentCulture));
        }

        public int Compare(Family family, Family other)
        {
            return LevenstheinDistance(family, other);
        }

        public bool Equals(Family family, Family other)
        {
            var equals = UsedComparers.Select(cmp => cmp.Equals(family, other));
            return family != null &&
                other != null &&
                equals.All(result => result == true);
        }

        public int GetHashCode(Family obj)
        {
            var hashCode = 673411578;
            foreach (var comparer in UsedComparers)
            {
                hashCode = hashCode * -1521134295 + comparer.GetHashCode(obj);
            }
            return hashCode;
        }

        public int LevenstheinDistance(Family model, Family other)
        {
            var distance = 0;
            foreach (var comparer in PropertyComparers)
            {
                distance += comparer.LevenstheinDistance(model, other);
            }
            return distance;
        }

        public string LevenstheinDistanceAsString(Family model, Family other)
        {
            var distance = LevenstheinDistance(model, other);
            return LevenstheinHelper.LevenstheinAsString(distance);
        }
    }
}
