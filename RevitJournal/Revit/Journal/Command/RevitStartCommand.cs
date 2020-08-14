using System.Collections.Generic;

namespace RevitJournal.Revit.Journal.Command
{
    public class RevitStartCommand
    {
        public IEnumerable<string> Commands
        {
            get { return new string[] { "'Dim Jrn", "Set Jrn = CrsJournalScript" }; }
        }
    }
}
