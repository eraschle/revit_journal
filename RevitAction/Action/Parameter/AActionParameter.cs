namespace RevitAction.Action
{
    public abstract class AActionParameter : IActionParameter
    {
        public string Name { get; private set; }

        public virtual string Value { get; set; } = string.Empty;

        public string DefaultValue { get; set; } = string.Empty;

        public bool BoolValue
        {
            get
            {
                if (bool.TryParse(Value, out var value) == false)
                {
                    value = false;
                }
                return value;
            }
        }

        public ParameterAction Action { get; }

        protected AActionParameter(string name, ParameterAction action)
        {
            Name = name;
            Action = action;
        }
    }
}
