using System;
using System.Collections.Generic;

namespace DataSource.Models.SharedParameters
{
    public class SharedGroup : IEquatable<SharedGroup>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SharedGroup);
        }

        public bool Equals(SharedGroup other)
        {
            return other != null &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public static bool operator ==(SharedGroup left, SharedGroup right)
        {
            return EqualityComparer<SharedGroup>.Default.Equals(left, right);
        }

        public static bool operator !=(SharedGroup left, SharedGroup right)
        {
            return !(left == right);
        }
    }
}