namespace RevitAction.Action
{
    public class ActionParameter : AActionParameter
    {
        public static ActionParameter Create(string name, bool initValue)
        {
            var value = initValue ? bool.TrueString : bool.FalseString;
            return Create(name, ParameterAction.Boolean, value, value);
        }

        public static ActionParameter Create(string name, string initValue = "", string defaultValue = null)
        {
            initValue = initValue is null ? string.Empty : initValue;
            return Create(name, ParameterAction.TextValue, initValue, defaultValue);
        }

        public static ActionParameter Create(string name, ParameterAction action, string initValue = "", string defaultValue = null)
        {
            initValue = initValue is null ? string.Empty : initValue;
            return new ActionParameter(name, action) { Value = initValue, DefaultValue = defaultValue };
        }

        public ActionParameter(string name, ParameterAction action) : base(name, action) { }
    }
}
