using RevitJournal.Journal.Command;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class AddIfNotCommandParameter : CommandParameterExternal
    {
        private const string DefaultParameterName = "Add if not exist";

        public AddIfNotCommandParameter(string jounralKey, string parameterName = DefaultParameterName)
            : base(jounralKey, parameterName, JournalParameterType.Boolean, true) { }

    }
}
