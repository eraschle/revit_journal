using DataSource.Model.Metadata;
using RevitJournal.Duplicate.Comparer.ParameterComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Duplicate.Comparer
{
    public class ParameterDuplicateComparer : ILevenstheinComparer<Parameter>, IModelDuplicateComparer<Parameter>
    {
        public static ICollection<IDuplicateComparer<Parameter>> AllComparers()
        {
            return new List<IDuplicateComparer<Parameter>>
            {
                new ParameterIdDublicateComparer(false),
                new ParameterNameDublicateComparer(true),
                new ParameterValueDublicateComparer(false),
                new ParameterValueTypeDublicateComparer(false),
                new ParameterParameterTypeDublicateComparer(true),
                new ParameterInstanceDublicateComparer(true),
            };
        }

        public static bool AllByName(string propertyName, out IDuplicateComparer<Parameter> comparer)
        {
            comparer = AllComparers().FirstOrDefault(cmp => cmp.PropertyName.Equals(propertyName, StringComparison.CurrentCulture));
            return comparer != null;
        }

        public ParameterDuplicateComparer() : this(AllComparers()) { }

        public ParameterDuplicateComparer(ICollection<IDuplicateComparer<Parameter>> comparers)
        {
            PropertyComparers = comparers;
        }

        public ICollection<IDuplicateComparer<Parameter>> PropertyComparers { get; set; }

        public ICollection<IDuplicateComparer<Parameter>> UsedComparers
        {
            get { return PropertyComparers.Where(cmp => cmp.UseComparer).ToList(); }
        }

        public bool HasByName(string propertyName, out IDuplicateComparer<Parameter> comparer)
        {
            comparer = ByName(propertyName);
            return comparer != null;
        }

        public IDuplicateComparer<Parameter> ByName(string propertyName)
        {
            return PropertyComparers.FirstOrDefault(cmp => cmp.PropertyName.Equals(propertyName, StringComparison.CurrentCulture));
        }

        public bool Equals(Parameter parameter, Parameter other)
        {
            var dublicateEquals = UsedComparers.Select(cmp => cmp.Equals(parameter, other));
            return parameter != null && other != null
                && dublicateEquals.All(cmpResult => cmpResult == true);
        }

        public int GetHashCode(Parameter obj)
        {
            var hashCode = -2129598428;
            foreach (var comparer in UsedComparers)
            {
                hashCode = hashCode * -1521134295 + comparer.GetHashCode(obj);
            }
            return hashCode;
        }

        public int LevenstheinDistance(Parameter model, Parameter other)
        {
            var distance = 0;
            foreach (var comparer in PropertyComparers)
            {
                distance += comparer.LevenstheinDistance(model, other);
            }
            return distance;
        }

        public string LevenstheinDistanceAsString(Parameter model, Parameter other)
        {
            var distance = LevenstheinDistance(model, other);
            return LevenstheinHelper.LevenstheinAsString(distance);
        }

        public int Compare(Parameter parameter, Parameter other)
        {
            return LevenstheinDistance(parameter, other);
        }
    }
}
