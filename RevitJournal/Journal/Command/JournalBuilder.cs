using RevitJournal.Journal.Command.Revit;
using RevitJournal.Journal.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitJournal.Journal.Command
{
    internal static class JournalBuilder
    {
        private static readonly IJournalCommand StartCommand = new RevitStartCommand();
        private static readonly IJournalCommand CloseCommand = new RevitCloseCommand();

        internal static string Build(JournalTask journalTask, out IList<JournalProcessCommand> commandLines)
        {
            commandLines = BuildLines(journalTask);
            var journal = new StringBuilder();
            foreach (var command in commandLines)
            {
                journal.Append(command.Command);
            }
            return journal.ToString();
        }

        private static IList<JournalProcessCommand> BuildLines(JournalTask journalTask)
        {
            var commandLines = new List<JournalProcessCommand>();
            commandLines = AddCommandLines(commandLines, StartCommand);
            foreach (var journalCommand in journalTask.Commands)
            {
                commandLines = AddCommandLines(commandLines, journalCommand);
            }
            commandLines = AddCommandLines(commandLines, CloseCommand);
            return commandLines;
        }

        private static List<JournalProcessCommand> AddCommandLines(List<JournalProcessCommand> commandLines, IJournalCommand journalCommand)
        {
            var lastLineNumber = commandLines.Count;
            foreach (var command in journalCommand.Commands)
            {
                var journalCmd = command;
                if (journalCommand.Equals(CloseCommand) == false)
                {
                    journalCmd += Environment.NewLine;
                }
                lastLineNumber++;
                commandLines.Add(new JournalProcessCommand(lastLineNumber, journalCmd));
            }
            if (journalCommand.Equals(CloseCommand) == false)
            {
                lastLineNumber++;
                commandLines.Add(new JournalProcessCommand(lastLineNumber, Environment.NewLine));
            }
            return commandLines;
        }
    }
}
