using Fam = DataSource.Model.Family;
using System;
using System.Text.RegularExpressions;
using Utilities.System;

namespace RevitJournalUI.MetadataUI.Models
{
    public class ParameterFileMetadataViewModel : ParameterViewModel
    {
        private static readonly Regex HasUnitRegex = new Regex(@"\d+\s[a-zA-Z]+$");
        private static readonly Regex UnitRegex = new Regex(@"[a-zA-Z]+$");

        private const string PrefixFormula = "= ";


        public bool HasUnitInValue { get { return HasUnitRegex.IsMatch(Parameter.Value); } }

        public bool HasUnit
        {
            get { return HasUnitInValue || string.IsNullOrWhiteSpace(Parameter.Unit) == false; }
        }

        public override string Value
        {
            get
            {
                if (HasUnitInValue)
                {
                    return Parameter.Value.Replace(Unit, string.Empty).Trim();
                }
                return Parameter.Value;
            }
            set
            {
                if (HasUnitInValue && value != null)
                {
                    value = string.Concat(value.Trim(), Constant.Space, Unit);
                }
                if (Parameter.Value != null && Parameter.Value.Equals(value, StringComparison.CurrentCulture)) { return; }

                Parameter.Value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public string Unit
        {
            get
            {
                if (HasUnitInValue == false) { return string.Empty; }

                var match = UnitRegex.Match(Parameter.Value);
                return match.Value;
            }
        }

        public string UnitType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Parameter.Unit))
                {
                    return ValueType;
                }
                return Parameter.Unit;
            }
        }

        public string Formula
        {
            get
            {
                if (Parameter.HasFormula(out var formula))
                {
                    return string.Concat(PrefixFormula, formula);
                }
                return Parameter.Formula;
            }
            set
            {
                if (Parameter.HasFormula(out var formula)
                    && string.Concat(PrefixFormula, formula).Equals(value, StringComparison.CurrentCulture)) { return; }

                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    value = value.Remove(0, PrefixFormula.Length);
                }
                Parameter.Formula = value;
                OnPropertyChanged(nameof(Formula));
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return Parameter.IsReadOnly 
                    || Parameter.HasFormula() 
                    || Fam.Parameter.NotSupportValueTypes.Contains(ValueType);
            }
        }
    }
}
