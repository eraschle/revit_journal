namespace RevitJournal.Journal.Command.Document
{
    public class DocumentSaveParameterBackup : CommandParameter
    {
        private const string DefaultParameterName = "Delete Backup";

        public DocumentSaveParameterBackup(string parameterName = DefaultParameterName)
            : base(parameterName, JournalParameterType.Boolean) { }

    }
}
