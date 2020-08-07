using System.Collections.Generic;

namespace RevitJournal.Journal.Command.Revit
{
    public class RevitStartCommand : AJournalCommand
    {
        public RevitStartCommand() : base("Start Revit", true) { }
        public override IEnumerable<string> Commands
        {
            get { return new string[] { "'Dim Jrn", "Set Jrn = CrsJournalScript" }; }
        }
    }
}
