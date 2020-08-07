using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public class CommandParameterExternalSelect : CommandParameterExternal
    {
        public IList<string> SelectableValues { get; private set; }

        public CommandParameterExternalSelect(string jounralKey, string name, bool isEnable, IList<string> selectableValues) 
            : base(jounralKey, name, JournalParameterType.Select, isEnable)
        {
            SelectableValues = selectableValues;
        }

    }
}
