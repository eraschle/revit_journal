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

        public ParameterKind Kind { get; }

        public string JournalKey { get; private set; }

        protected AActionParameter(string name, string journalKey, ParameterKind kind)
        {
            Name = name;
            JournalKey = journalKey;
            Kind = kind;
        }
    }
}
