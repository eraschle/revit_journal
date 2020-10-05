namespace RevitJournal.Tasks.Options.Parameter
{
    public class TaskOption<TValue>
    {
        public TaskOption(TValue defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public virtual TValue Value { get; set; }

        public bool HasValue(out TValue value)
        {
            value = Value;
            return value != null;
        }

        public TValue DefaultValue { get; }
    }
}
