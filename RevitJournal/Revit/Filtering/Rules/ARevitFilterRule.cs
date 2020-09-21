using RevitJournal.Library.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Revit.Filtering.Rules
{
    public abstract class ARevitFilterRule<TSource> : IFilterRule<TSource>, IEquatable<ARevitFilterRule<TSource>>
    {
        public string Name { get; set; }

        protected HashSet<FilterValue> filterValues { get; } = new HashSet<FilterValue>();

        public IEnumerable<FilterValue> Values
        {
            get { return filterValues.OrderBy(value => value.Name); }
        }

        public IEnumerable<FilterValue> CheckedValues
        {
            get { return filterValues.Where(value => value.IsChecked); }
        }

        public bool HasCheckedValues
        {
            get { return filterValues.Any(value => value.IsChecked); }
        }

        public abstract void AddValue(TSource source);

        public abstract bool Allowed(TSource source);

        public override bool Equals(object obj)
        {
            return Equals(obj as IFilterRule<TSource>);
        }

        public bool Equals(IFilterRule<TSource> other)
        {
            return other != null &&
                   Name == other.Name;
        }

        public bool Equals(ARevitFilterRule<TSource> other)
        {
            return Equals(other as IFilterRule<TSource>);
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        protected ARevitFilterRule(string name)
        {
            Name = name;
        }

        public static bool operator ==(ARevitFilterRule<TSource> left, ARevitFilterRule<TSource> right)
        {
            return EqualityComparer<ARevitFilterRule<TSource>>.Default.Equals(left, right);
        }

        public static bool operator !=(ARevitFilterRule<TSource> left, ARevitFilterRule<TSource> right)
        {
            return !(left == right);
        }
    }
}
