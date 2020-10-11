using DataSource.Models;
using System;
using System.Collections.Generic;

namespace RevitJournal.Library.Filtering
{
    public interface IFilterRule<TSource> : IEquatable<IFilterRule<TSource>>
    {
        string Name { get; set; }

        IEnumerable<FilterValue> Values { get; }

        IEnumerable<FilterValue> CheckedValues { get; }

        bool HasCheckedValues { get; }

        bool Allowed(TSource source);

        void AddValue(TSource source);
    }
}
