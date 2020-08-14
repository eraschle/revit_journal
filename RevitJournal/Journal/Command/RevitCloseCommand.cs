using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public class RevitCloseCommand
    {
        public IEnumerable<string> Commands
        {
            get
            {
                return new string[] {
                    JournalBuilder.Build("SystemMenu", "ID_APP_EXIT") };
            }
        }
    }
}
