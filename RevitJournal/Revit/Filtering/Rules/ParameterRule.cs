using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using RevitJournal.Library.Filtering;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace RevitJournal.Revit.Filtering.Rules
{
    public class ParameterRule : ARevitListFilterRule<RevitFamily>
    {
        public const string RuleKey = "RevitParameter";

        public ParameterRule(string name) : base(name) { }

        protected override IEnumerable<FilterValue> GetValue(RevitFamily family)
        {
            if (family is null || family.Metadata is null) { return null; }

            return family.Metadata.Parameters.Where(par => IsBoolType(par))
                                             .Select(par => new FilterValue(par));
        }

        private bool IsBoolType(Parameter parameter)
        {
            if (parameter is null) { return false; }

            var valueType = parameter.ValueType;
            return valueType != null && StringUtils.Equals(valueType, Parameter.BooleanValueType);
        }
    }
}
