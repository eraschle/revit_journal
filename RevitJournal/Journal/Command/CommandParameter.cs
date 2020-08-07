namespace RevitJournal.Journal.Command
{
    public class CommandParameter : ICommandParameter
    {
        public CommandParameter(string parameterName, JournalParameterType parameterType) 
            : this(parameterName, parameterType, true) { }

        public CommandParameter(string parameterName, JournalParameterType parameterType, bool isEditable)
        {
            Name = parameterName;
            ParameterType = parameterType;
            Value = string.Empty;
            if (ParameterType == JournalParameterType.Boolean)
            {
                Value = bool.FalseString;
            }
            IsEnable = isEditable;
        }

        public string JournalKey { get; } = string.Empty;

        public JournalParameterType ParameterType { get; }

        public string Name { get; }

        public virtual string Value { get; set; }

        public bool IsEnable { get; }
    }
}
