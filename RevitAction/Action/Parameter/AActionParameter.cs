using Newtonsoft.Json;

namespace RevitAction.Action
{
    public abstract class AActionParameter : IActionParameter
    {
        public string Name { get; set; }

        public virtual string Value { get; set; } = string.Empty;

        [JsonIgnore]
        public string DefaultValue { get; set; } = string.Empty;

        [JsonIgnore]
        public ParameterKind Kind { get; }

        public string JournalKey { get; }

        public bool IsJournalParameter
        {
            get { return string.IsNullOrWhiteSpace(JournalKey) == false; }
        }

        protected AActionParameter(string name, string journalKey, ParameterKind kind)
        {
            Name = name;
            JournalKey = journalKey;
            Kind = kind;
        }

        public bool GetBoolValue()
        {
            if (bool.TryParse(Value, out var value) == false)
            {
                value = false;
            }
            return value;
        }

        public int GetIntValue()
        {
            if (int.TryParse(Value, out var value) == false)
            {
                if (int.TryParse(DefaultValue, out value) == false)
                {
                    value = 1;
                }
            }
            return value;
        }

        public virtual string GetJournalValue()
        {
            return Value;
        }

        public virtual void SetJournalValue(string journalValue)
        {
            Value = journalValue;
        }
    }
}
