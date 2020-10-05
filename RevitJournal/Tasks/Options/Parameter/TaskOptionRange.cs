namespace RevitJournal.Tasks.Options.Parameter
{
    public class TaskOptionRange<TValue> : TaskOption<TValue>
    {
        public TaskOptionRange(TValue defaultValue, TValue minValue, TValue maxValue) : base(defaultValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public TValue MinValue { get; }

        public TValue MaxValue { get; }
    }
}
