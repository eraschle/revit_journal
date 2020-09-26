namespace RevitJournal.Revit.Command
{
    internal static class JournalBuilder
    {
        internal static string Build(string start, string commandId)
        {
            return string.Concat("Jrn.Command \"", start, "\" , \" , ", commandId, "\"");
        }
    }
}
