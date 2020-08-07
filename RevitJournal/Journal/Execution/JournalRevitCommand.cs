namespace RevitJournal.Journal.Execution
{
    public class JournalRevitCommand
    {
        public int LineNumber { get; private set; }

        internal bool HasLineNumber { get { return LineNumber > 0; } }

        public string Command { get; private set; }

        internal bool IsJournalStart { get; set; } = false;

        internal bool IsJournalExit { get; set; } = false;

        internal bool IsJournalCommandStop { get; set; } = false;

        internal JournalRevitCommand(string command) : this(-1, command) { }

        internal JournalRevitCommand(int lineNumber, string command)
        {
            LineNumber = lineNumber;
            Command = command;
        }
    }
}
