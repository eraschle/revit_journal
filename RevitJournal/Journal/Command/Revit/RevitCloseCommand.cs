using System.Collections.Generic;

namespace RevitJournal.Journal.Command.Revit
{
    public class RevitCloseCommand : AJournalCommand
    {
        public RevitCloseCommand() : base("Close Revit", true) { }

        public override IEnumerable<string> Commands
        {
            get
            {
                return new string[] {
                    JournalCommandBuilder.Build("SystemMenu", "ID_APP_EXIT") };
            }
        }
    }
}
