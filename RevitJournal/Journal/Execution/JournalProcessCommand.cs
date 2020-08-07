namespace RevitJournal.Journal.Execution
{
    public class JournalProcessCommand
    {
        public int LineNumber { get; private set; }

        public string Command { get; private set; }

        internal JournalProcessCommand(int lineNumber, string command)
        {
            LineNumber = lineNumber;
            Command = command;
        }
    }
}
