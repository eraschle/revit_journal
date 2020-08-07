namespace RevitJournal.Journal.Command
{
    public enum JournalParameterType
    {
        Info, String, Boolean, TextFile, ImageFile, Folder, List, Select
    }

    public interface ICommandParameter
    {
        JournalParameterType ParameterType { get; }

        string JournalKey { get; }

        string Name { get; }

        string Value { get; set; }

        bool IsEnable { get; }
    }
}
