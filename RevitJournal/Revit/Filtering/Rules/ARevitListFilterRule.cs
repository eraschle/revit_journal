using RevitJournal.Library.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Revit.Filtering.Rules
{
    public abstract class ARevitListFilterRule<TSource> : ARevitFilterRule<TSource>
    {
        protected ARevitListFilterRule(string name) : base(name) { }

        private bool HasChecked(out ISet<FilterValue> values)
        {
            values = CheckedValues.ToHashSet();
            return values.Count > 0;
        }

        public override void AddValue(TSource source)
        {
            var values = GetValue(source);
            if(values is null) { return; }

            foreach (var value in values)
            {
                if (FilterValues.Contains(value)) { continue; }

                FilterValues.Add(value);
            }
        }

        public override bool Allowed(TSource source)
        {
            if (HasChecked(out var checkedValues) == false) { return true; }

            var values = GetValue(source);
            return values is object && checkedValues.Any(value => values.Contains(value));
        }

        protected abstract IEnumerable<FilterValue> GetValue(TSource source);

    }
}
