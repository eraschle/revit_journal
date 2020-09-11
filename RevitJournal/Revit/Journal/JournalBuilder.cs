using RevitAction.Action;
using RevitJournal.Revit.Journal.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitJournal.Revit.Journal
{
    internal static class JournalBuilder
    {
        private static readonly RevitStartCommand startRevit = new RevitStartCommand();
        private static readonly RevitCloseCommand closeRevit = new RevitCloseCommand();


        internal static string Build(IEnumerable<ITaskAction> actions)
        {
            var journalLines = new StringBuilder();
            AddLines(ref journalLines, startRevit.Commands);
            foreach (var action in actions)
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
            var parameters = command.Parameters.Where(par => par.IsJournalParameter);
            var journalData = new StringBuilder();
            journalData.Append($"Jrn.Data \"APIStringStringMapJournalData\", {parameters.Count()}");
            foreach (var parameter in parameters)
            {
                journalData.Append($", \"{parameter.JournalKey}\", \"{parameter.GetJournalValue()}\"");
            }

            return new string[]
            {
                $"Jrn.RibbonEvent \"Execute external command:{command.Id}:{command.TaskInfo.TaskNamespace}\"",
                journalData.ToString()
            };
        }
    }
}
