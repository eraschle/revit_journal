using RevitJournal.Library.Filtering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Utilities.System;

namespace RevitJournal.Revit.Filtering.Rules
{
    public abstract class ARevitFilterRule<TSource> : IFilterRule<TSource>, IEquatable<ARevitFilterRule<TSource>>
    {
        public string Name { get; set; }

        private readonly IComparer<string> comparer;

        protected HashSet<FilterValue> FilterValues { get; } = new HashSet<FilterValue>();

        public IEnumerable<FilterValue> Values
        {
            get { return FilterValues.OrderBy(value => value.Name, comparer); }
        }

        public IEnumerable<FilterValue> CheckedValues
        {
            get { return FilterValues.Where(value => value.IsChecked); }
        }

        public bool HasCheckedValues
        {
            get { return FilterValues.Any(value => value.IsChecked); }
        }

        protected ARevitFilterRule(string name, string defaultValue = null)
        {
            Name = name;
            comparer = new ValueComparer(defaultValue);
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

        public static bool operator ==(ARevitFilterRule<TSource> left, ARevitFilterRule<TSource> right)
        {
            return EqualityComparer<ARevitFilterRule<TSource>>.Default.Equals(left, right);
        }

        public static bool operator !=(ARevitFilterRule<TSource> left, ARevitFilterRule<TSource> right)
        {
            return !(left == right);
        }

        internal class ValueComparer : IComparer<string>
        {
            private readonly string defaultValue;

            internal ValueComparer(string defaultValue)
            {
                this.defaultValue = defaultValue;
            }

            public int Compare(string value, string other)
            {
                var compare = 0;
                if (string.IsNullOrWhiteSpace(defaultValue) == false)
                {
                    if (StringUtils.Equals(value, defaultValue))
                    {
                        compare = -1;
                    }
                    else if (StringUtils.Equals(other, defaultValue))
                    {
                        compare = 1;
                    }
                }
                if (compare != 0) { return compare; }

                return StringUtils.Compare(value, other);
            }
        }
    }
}
