using System;
using System.Collections.Generic;

namespace DataSource.Model.Catalog
{
    public class ACatalog : IModel, IEquatable<ACatalog>
    {
        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} [{Id}]";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ACatalog);
        }

        public bool Equals(ACatalog other)
        {
            return other != null &&
                   Id == other.Id &&
                   Name == other.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -1919740922;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public static bool operator ==(ACatalog left, ACatalog right)
        {
            return EqualityComparer<ACatalog>.Default.Equals(left, right);
        }

        public static bool operator !=(ACatalog left, ACatalog right)
        {
            return !(left == right);
        }
    }
}
