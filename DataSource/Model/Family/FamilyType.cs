using DataSource.Comparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataSource.Model.Family
{
    public class FamilyType : IComparable<FamilyType>, IModel, IEquatable<FamilyType>
    {
        public string Name { get; set; }

        private readonly List<Parameter> _Parameters = new List<Parameter>();
        public IList<Parameter> Parameters { get { return _Parameters; } }

        public Parameter ByName(string name)
        {
            return _Parameters.FirstOrDefault(par => par.IsParameterName(name));
        }

        public bool HasByName(string name, out Parameter parameter)
        {
            parameter = ByName(name);
            return parameter != null;
        }

        public void AddParameter(Parameter parameter)
        {
            if (_Parameters.Contains(parameter)) { return; }

            _Parameters.Add(parameter);
            _Parameters.Sort();
        }

        public int CompareTo(FamilyType other)
        {
            if (other is null) { return -1; }

            return Name.CompareTo(other.Name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FamilyType);
        }

        public bool Equals(FamilyType other)
        {
            return other != null 
                && Name == other.Name 
                && Enumerable.SequenceEqual(Parameters, other.Parameters);
        }

        public override int GetHashCode()
        {
            var hashCode = 497090031;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<Parameter>>.Default.GetHashCode(Parameters);
            return hashCode;
        }

        public static bool operator ==(FamilyType left, FamilyType right)
        {
            return EqualityComparer<FamilyType>.Default.Equals(left, right);
        }

        public static bool operator !=(FamilyType left, FamilyType right)
        {
            return !(left == right);
        }

        public static bool operator <(FamilyType left, FamilyType right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(FamilyType left, FamilyType right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(FamilyType left, FamilyType right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(FamilyType left, FamilyType right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
