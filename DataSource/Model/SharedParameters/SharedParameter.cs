using System;
using System.Collections.Generic;

namespace DataSource.Models.SharedParameters
{
    public class SharedParameter : IEquatable<SharedParameter>
    {
        public string Guid { get; set; }

        public string Name { get; set; }

        public string Datatype { get; set; }

        public string DataCategory { get; set; }

        public SharedGroup Group { get; set; }

        public bool Visible { get; set; }

        public string Description { get; set; }

        public bool UserModifiable { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SharedParameter);
        }

        public bool Equals(SharedParameter other)
        {
            return other != null &&
                   Guid == other.Guid &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -470705902;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Guid);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
