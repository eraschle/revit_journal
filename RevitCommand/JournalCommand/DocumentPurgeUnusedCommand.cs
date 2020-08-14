using RevitAction.Action;
using System.Collections.Generic;

namespace RevitCommand.JournalCommand
{
    public class DocumentPurgeUnusedCommand : ATaskAction
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
    }
}
