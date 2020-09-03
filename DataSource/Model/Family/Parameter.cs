using System;
using System.Collections.Generic;

namespace DataSource.Model.Family
{
    public class Parameter : IComparable<Parameter>, IEquatable<Parameter>
    {
        public const string SystemParameterType = "system";
        public const string SharedParameterType = "shared";
        public const string CostumParameterType = "custom";

        public const string BooleanValueType = "Ja/Nein";
        public const string FamilyTypeValueType = "Family Type";
        public const string MaterialValueType = "Material";
        public const string LastklassifizierungValueType = "Lastklassifizierung";

        public static IList<string> NotSupportValueTypes { get; }
            = new List<string> 
            {
                null, string.Empty,
                MaterialValueType,
                FamilyTypeValueType,
                LastklassifizierungValueType
            };

        public string Id { get; set; }

        public string Name { get; set; }

        public string BuiltIn { get; set; }

        public string Value { get; set; }

        public string ValueType { get; set; }

        public string Unit { get; set; }

        public string ParameterType { get; set; }

        internal bool IsParameterName(string name)
        {
            return Name.Equals(name, StringComparison.CurrentCulture);
        }

        public bool IsSytsemParameterType() { return IsParameterType(SystemParameterType); }

        public bool IsSharedType() { return IsParameterType(SharedParameterType); }

        private bool IsParameterType(string parameterType)
        {
            return ParameterType.Equals(parameterType, StringComparison.CurrentCulture);
        }

        public bool IsInstance { get; set; } = false;

        public bool IsReadOnly { get; set; } = false;

        public string Formula { get; set; } = null;

        public bool HasFormula()
        {
            return string.IsNullOrWhiteSpace(Formula) == false;
        }

        public bool HasFormula(out string formula)
        {
            formula = Formula;
            return HasFormula();
        }

        public override string ToString()
        {
            return $"{Name} ({Value}) [{ParameterType}]";
        }

        public int CompareTo(Parameter other)
        {
            if (other is null) { return -1; }

            if (IsInstance != other.IsInstance)
            {
                return IsInstance ? 1 : -1;
            }
            if (ParameterType != other.ParameterType)
            {
                if (IsSytsemParameterType() || (IsSharedType() && other.IsSytsemParameterType() == false))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            return Name.CompareTo(other.Name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Parameter);
        }

        public bool Equals(Parameter other)
        {
            return other != null
                && Id == other.Id
                && Name == other.Name
                && ValueType == other.ValueType
                && IsInstance == other.IsInstance;
        }

        public override int GetHashCode()
        {
            var hashCode = -691830078;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ValueType);
            hashCode = hashCode * -1521134295 + IsInstance.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Parameter left, Parameter right)
        {
            return EqualityComparer<Parameter>.Default.Equals(left, right);
        }

        public static bool operator !=(Parameter left, Parameter right)
        {
            return !(left == right);
        }

        public static bool operator <(Parameter left, Parameter right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Parameter left, Parameter right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Parameter left, Parameter right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Parameter left, Parameter right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
