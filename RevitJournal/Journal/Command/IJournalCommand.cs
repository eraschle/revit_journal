using DataSource.Model.FileSystem;
using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public interface IJournalCommand
    {
        string Name { get; }

        bool HasParameters { get; }

        IList<ICommandParameter> Parameters { get; }

        bool IsDefault { get; }

        bool DependsOnCommand(IJournalCommand command);

        void PreExecutionTask(RevitFamily family);

        IEnumerable<string> Commands { get; }

        void PostExecutionTask(JournalResult result);
    }
}
