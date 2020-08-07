using System.Collections.Generic;

namespace RevitJournal.Journal.Command.Document
{
    public class DocumentPurgeUnusedCommand : AJournalCommand
    {
        public DocumentPurgeUnusedCommand() : base("Purge unused") { }
        public override IEnumerable<string> Commands
        {
            get
            {
                // Jrn.Command "Ribbon" , " , ID_PURGE_UNUSED"
                // Jrn.PushButton "Modal ,  , Dialog_Revit_PurgeUnusedTree" , "OK, IDOK"
                return new string[] {
                    JournalCommandBuilder.Build("Ribbon", "ID_PURGE_UNUSED"),
                    "Jrn.PushButton \"Modal ,  , Dialog_Revit_PurgeUnusedTree\" , \"OK, IDOK\""};
            }
        }

        public override bool DependsOnCommand(IJournalCommand command)
        {
            return command is DocumentSaveCommand || command is DocumentSaveCommand;
        }
    }
}
