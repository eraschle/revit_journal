using DataSource.Model;
using System;
using System.Collections.Generic;

namespace RevitJournal.Library.Filtering
{
    public class FilterValue : IEquatable<FilterValue>
    {
        public bool IsChecked { get; set; } = false;

        public string Name { get; private set; }

        public FilterValue(string name)
        {
            Name = name;
        }

        public FilterValue(IModel model)
        {
            if(model is null) { throw new ArgumentNullException(nameof(model)); }

            Name = model.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FilterValue);
        }

        public bool Equals(FilterValue other)
        {
            return other != null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public static bool operator ==(FilterValue left, FilterValue right)
        {
            return EqualityComparer<FilterValue>.Default.Equals(left, right);
        }

        public static bool operator !=(FilterValue left, FilterValue right)
        {
            return !(left == right);
        }
    }
}
