using System;

namespace RevitJournal.Journal.Command
{
    public class CommandParameterInfo : CommandParameter
    {
        private readonly Func<string> ValueFunc;

        public CommandParameterInfo(string parameterName, Func<string> valueFunc)
            : base(parameterName, JournalParameterType.Info, false)
        {
            ValueFunc = valueFunc;
        }

        public override string Value
        {
            get { return ValueFunc.Invoke(); }
            set {  }
        }
    }
}
