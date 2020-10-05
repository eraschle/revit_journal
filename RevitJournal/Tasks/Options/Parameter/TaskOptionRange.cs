namespace RevitJournal.Tasks.Options.Parameter
{
    public class TaskOptionRange : TaskOption<double>
    {
        public TaskOptionRange(double defaultValue, double minValue, double maxValue) : base(defaultValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public double MinValue { get; }

        public double MaxValue { get; }
    }
}
