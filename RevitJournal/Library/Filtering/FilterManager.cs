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

        public IFilterRule<TSource> GetRule(string key)
        {
            return FilterRules.ContainsKey(key) ? FilterRules[key] : null;
        }

        public IEnumerable<FilterValue> GetValues(string key)
        {
            return FilterRules.ContainsKey(key) ? FilterRules[key].Values : new List<FilterValue>();
        }

        public void ClearFilter()
        {
            FilterRules.Clear();
        }

        public bool NoFilter()
        {
            return FilterRules.Values.All(rule => rule.HasCheckedValues == false);
        }

        public bool FileFilter(TSource source)
        {
            if (source is null) { throw new ArgumentNullException(nameof(source)); }

            var allowed = true;
            var iterator = FilterRules.Values.GetEnumerator();
            while (allowed && iterator.MoveNext())
            {
                allowed &= iterator.Current.Allowed(source);
            }
            return allowed;
        }
    }
}
