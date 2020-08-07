using System.Collections.Generic;

namespace RevitJournal.Journal.Command.Document
{
    public class DocumentSaveCommand : AJournalCommand
    {
        public DocumentSaveCommand() : base("Save")
        {
            Parameters.Add(new DocumentSaveParameterBackup());
        }

        public override IEnumerable<string> Commands
        {
            get { return new string[] { JournalCommandBuilder.Build("Ribbon", "ID_REVIT_FILE_SAVE") }; }
        }

        public override void PostExecutionTask(JournalResult result)
        {
            if (Parameters[0].Value.Equals(bool.FalseString)
                || result.HasError()) { return; }


            foreach (var backup in result.Original.Backups)
            {
                backup.Delete();
            }
        }
    }
}
