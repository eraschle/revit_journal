using RevitJournal.Library.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Revit.Filtering.Rules
{
    public abstract class ARevitModelFilterRule<TSource> : ARevitFilterRule<TSource>
    {
        protected ARevitModelFilterRule(string name) : base(name) { }

        protected bool HasChecked(out ISet<FilterValue> values)
        {
            values = CheckedValues.ToHashSet();
            return values.Count > 0;
        }

        public override void AddValue(TSource source)
        {
            var value = GetValue(source);
            if (value is null || filterValues.Contains(value)) { return; }

            filterValues.Add(value);
        }

        public override bool Allowed(TSource source)
        {
            if (HasChecked(out var checkedValues) == false) { return true; }

            var value = GetValue(source);
            return value is object && checkedValues.Contains(value);
        }

        protected abstract FilterValue GetValue(TSource source);

    }
}
