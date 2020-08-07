using System;
using System.Collections.Generic;
using System.Text;

namespace RevitJournal.Journal.Command
{
    internal static class JournalCommandBuilder
    {
        internal const string JournalCommand = "Jrn.Command";

        internal static string Build(string start, string commandId)
        {
            return string.Concat(JournalCommand, " \"", start, "\" , \" , ", commandId, "\"");
        }
        internal static string BuildExternalAddinLaunch(Guid addinId, string commandNamespace)
        {
            return $"Jrn.RibbonEvent \"Execute external command:{addinId}:{commandNamespace}\"";
        }

        internal static string BuildExternalAddinCommandData(IDictionary<string, string> commandData)
        {
            var journalData = new StringBuilder();
            journalData.Append($"Jrn.Data \"APIStringStringMapJournalData\", {commandData.Keys.Count}");
            foreach (var key in commandData.Keys)
            {
                journalData.Append($", \"{key}\", \"{commandData[key]}\"");
            }
            return journalData.ToString();
        }
    }
}
