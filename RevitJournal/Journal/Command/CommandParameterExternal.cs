namespace RevitJournal.Journal.Command
{
    public class CommandParameterExternal : ICommandParameter
    {
        public CommandParameterExternal(string jounralKey, string name, JournalParameterType parameterType, bool isEnable)
        {
            JournalKey = jounralKey;
            Name = name;
            ParameterType = parameterType;
            Value = string.Empty;
            if(ParameterType == JournalParameterType.Boolean)
            {
                Value = bool.FalseString;
            }
            IsEnable = isEnable;
        }

        public string JournalKey { get; private set; }
        
        public JournalParameterType ParameterType { get; }

        public string Name { get; }

        public string Value { get; set; }

        public bool IsEnable { get; }
    }
}
