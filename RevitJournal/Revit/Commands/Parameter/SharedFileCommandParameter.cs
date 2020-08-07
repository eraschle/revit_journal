using RevitJournal.Journal.Command;
using RevitJournal.Revit.SharedParameters;
using System.Collections.Generic;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class SharedFileCommandParameter : CommandParameterExternal
    {
        private const string DefaultParameterName = "File";

        public SharedFileCommandParameter(string jounralKey, string parameterName = DefaultParameterName)
            : base(jounralKey, parameterName, JournalParameterType.TextFile, true) { }

        public IList<SharedParameter> GetSharedParameters()
        {
            var names = SharedParameterReader.GetParameters(Value);
            if (names is null)
            {
                names = new List<SharedParameter>();
            }
            return names;
        }
    }
}
