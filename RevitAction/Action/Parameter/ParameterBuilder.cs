namespace RevitAction.Action
{
    public class ParameterBuilder
    {
        public static ActionParameter Bool(string name, bool initValue)
        {
            return BoolJournal(name, string.Empty, initValue);
        }

        public static ActionParameter BoolJournal(string name, string journalKey, bool initValue)
        {
            var value = initValue ? bool.TrueString : bool.FalseString;
            return CreateJournal(name, journalKey, ParameterKind.Boolean, value, value);
        }

        public static ActionParameter Text(string name, string initValue = "", string defaultValue = null)
        {
            return TextJournal(name, string.Empty, initValue, defaultValue);
        }

        public static ActionParameter TextJournal(string name, string journalKey, string initValue = "", string defaultValue = null)
        {
            initValue = initValue is null ? string.Empty : initValue;
            return CreateJournal(name, journalKey, ParameterKind.TextValue, initValue, defaultValue);
        }
        public static ActionParameter Create(string name, ParameterKind action, string initValue = "", string defaultValue = null)
        {
            return CreateJournal(name, string.Empty, action, initValue, defaultValue);
        }

        public static ActionParameter CreateJournal(string name, string journalKey, ParameterKind action, string initValue = "", string defaultValue = null)
        {
            initValue = initValue is null ? string.Empty : initValue;
            return new ActionParameter(name, journalKey, action) { Value = initValue, DefaultValue = defaultValue };
        }
    }
}
