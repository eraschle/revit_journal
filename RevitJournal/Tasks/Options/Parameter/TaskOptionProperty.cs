using System;
using System.Reflection;

namespace RevitJournal.Tasks.Options.Parameter
{
    public class TaskOptionProperty<TValue> : TaskOption<TValue>
    {
        private readonly PropertyInfo property;
        private readonly object source;

        public TaskOptionProperty(TValue defaultValue, object sourceObject, string propertyName) : base(defaultValue)
        {
            source = sourceObject ?? throw new ArgumentNullException(nameof(sourceObject));
            property = source.GetType().GetProperty(propertyName);
        }

        public override TValue Value
        {
            get
            {
                var value = property.GetValue(source);
                return (TValue)value;
            }
            set { property.SetValue(source ,value); }
        }
    }
}
