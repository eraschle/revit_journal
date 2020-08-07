using RevitCommand.Families;
using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public interface IJournalCommandExternal : IJournalCommand
    {
        IDictionary<string, string> JournalData { get; }

        IRevitExternalCommandData CommandData { get; }
    }
}
