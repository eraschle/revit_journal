namespace RevitJournal.Tasks.Options.Parameter
{
    public interface ITaskOption<TValue>
    {
        TValue Value { get; set; }

        bool HasValue(out TValue value);
        
        TValue DefaultValue { get; }
    }
}
