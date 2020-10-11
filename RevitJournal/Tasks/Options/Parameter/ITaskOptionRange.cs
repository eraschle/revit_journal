namespace RevitJournal.Tasks.Options.Parameter
{
    public interface ITaskOptionRange : ITaskOption<double>
    {
        double MinValue { get; }

        double MaxValue { get; }
    }
}
