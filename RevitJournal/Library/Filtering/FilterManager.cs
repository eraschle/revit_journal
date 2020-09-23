using DataSource.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Library.Filtering
{
    public class FilterManager<TSource> where TSource : class
    {
        public IDictionary<string, IFilterRule<TSource>> FilterRules { get; } = new Dictionary<string, IFilterRule<TSource>>();

        public void AddRule(string key, IFilterRule<TSource> rule)
        {
            if (FilterRules.ContainsKey(key)) { return; }

            FilterRules.Add(key, rule);
        }

        public void AddValue(TSource source)
        {
            foreach (var rule in FilterRules.Values)
            {
                rule.AddValue(source);
            }
        }

        public IEnumerable<IFilterRule<TSource>> GetCheckedRules()
        {
            foreach (var rule in FilterRules.Values)
            {
                if (rule.HasCheckedValues == false) { continue; }

                yield return rule;
            }
        }

        public bool HasRule(string key, out IFilterRule<TSource> filter)
        {
            filter = null;
            if (FilterRules.ContainsKey(key))
            {
                filter = FilterRules[key];
            }
            return filter is object;
        }

        public IEnumerable<FilterValue> GetValues(string key)
        {
            return FilterRules.ContainsKey(key) ? FilterRules[key].Values : new List<FilterValue>();
        }

        public void ClearFilter()
        {
            FilterRules.Clear();
        }

        public bool HasFilters()
        {
            return FilterRules.Values.Any(rule => rule.HasCheckedValues);
        }

        public bool FileFilter(TSource source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }
            if(HasFilters() == false) { return true; }

            var allowed = true;
            var rules = FilterRules.Values.GetEnumerator();
            while (allowed && rules.MoveNext())
            {
                allowed &= rules.Current.Allowed(source);
            }
            return allowed;
        }
    }
}
