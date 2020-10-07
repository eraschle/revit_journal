using System;
using System.Collections.Generic;

namespace RevitJournal.Tasks.Options.Parameter
{
    public class TaskOptionSelect<TValue> : TaskOption<TValue>
    {
        public TaskOptionSelect(TValue defaultValue, IEnumerable<TValue> values) : base(defaultValue)
        {
            if(values is null) { throw new ArgumentNullException(nameof(values)); }

            Values = values;
        }

        public IEnumerable<TValue> Values { get; }
    }
}
