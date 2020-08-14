using System;

namespace RevitAction.Action
{
    public class ActionParameterInfo : ActionParameter
    {
        private readonly Func<string> valuFunc;

        public ActionParameterInfo(string name, Func<string> value)
            : base(name, string.Empty, ParameterKind.InfoDynamic)
        {
            valuFunc = value;
        }

        public override string Value
        {
            get { return valuFunc.Invoke(); }
            set { }
        }
    }
}
