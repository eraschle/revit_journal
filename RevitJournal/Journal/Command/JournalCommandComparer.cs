using RevitJournal.Journal.Command.Document;
using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public class JournalCommandComparer : IComparer<IJournalCommand>
    {
        public int Compare(IJournalCommand command, IJournalCommand other)
        {
            if (command is DocumentOpenCommand) { return -1; }

            if (command.DependsOnCommand(other)) { return -1; }
            if (other.DependsOnCommand(command)) { return 1; }

            return command.Name.CompareTo(other.Name);
        }
    }
}
