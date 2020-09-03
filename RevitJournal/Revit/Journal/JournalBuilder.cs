using RevitAction.Action;
using RevitJournal.Revit.Journal.Command;
using RevitJournal.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitJournal.Revit.Journal
{
    internal static class JournalBuilder
    {
        private static readonly RevitStartCommand startRevit = new RevitStartCommand();
        private static readonly RevitCloseCommand closeRevit = new RevitCloseCommand();

        internal static string Build(RevitTask task)
        {
            var journalLines = new StringBuilder();
            AddLines(ref journalLines, startRevit.Commands);
            foreach (var action in task.Actions)
            {
                if (action is ITaskActionJournal journal)
                {
                    AddJournal(ref journalLines, journal);
                }
                else if (action is ITaskActionCommand command)
                {
                    AddCommand(ref journalLines, command);
                }
            }
            AddLines(ref journalLines, closeRevit.Commands);
            return journalLines.ToString();
        }

        private static void AddCommand(ref StringBuilder commands, ITaskActionCommand action)
        {
            var commandLines = BuildCommand(action);
            AddLines(ref commands, commandLines);
        }

        private static void AddJournal(ref StringBuilder commands, ITaskActionJournal action)
        {
            AddLines(ref commands, action.Commands);
        }

        private static void AddLines(ref StringBuilder commands, IEnumerable<string> commandLines)
        {
            foreach (var command in commandLines)
            {
                commands.AppendLine(command);
            }
            commands.AppendLine(Environment.NewLine);
        }

        internal static string Build(string start, string commandId)
        {
            return string.Concat("Jrn.Command \"", start, "\" , \" , ", commandId, "\"");
        }

        private static IEnumerable<string> BuildCommand(ITaskActionCommand command)
        {
            var parameters = command.Parameters;
            var data = new StringBuilder();
            data.Append($"Jrn.Data \"APIStringStringMapJournalData\", {parameters.Count}");
            foreach (var parameter in parameters)
            {
                data.Append($", \"{parameter.JournalKey}\", \"{parameter.Value}\"");
            }

            return new string[]
            {
                $"Jrn.RibbonEvent \"Execute external command:{command.Id}:{command.Namespace}\"",
                data.ToString()
            };
        }
    }
}
