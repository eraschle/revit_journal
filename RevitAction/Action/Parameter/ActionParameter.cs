namespace RevitAction.Action
{
    public class ActionParameter : AActionParameter
    {
        public static ActionParameter Bool(string name, string journalKey, bool initValue)
        {
            var value = initValue ? bool.TrueString : bool.FalseString;
            return Create(name, journalKey, ParameterKind.Boolean, value, value);
        }

        public static ActionParameter Text(string name, string journalKey, string initValue = null, string defaultValue = null)
        {
            return Create(name, journalKey, ParameterKind.TextValue, initValue, defaultValue);
        }

        public static ActionParameter Create(string name, string journalKey, ParameterKind parameterKind, string initValue = null, string defaultValue = null)
        {
            return new ActionParameter(name, journalKey, parameterKind)
            {
                Value = initValue ?? string.Empty,
                DefaultValue = defaultValue
            };
        }

        public ActionParameter(string name, string journalKey, ParameterKind parameterKind)
            : base(name, journalKey, parameterKind) { }
    }
}
