using System;

namespace RevitAction.Action
{
    public class ActionParameterInfo : ActionParameter
    {
        private readonly Func<string> ValuFunc;

        public ActionParameterInfo(string name, Func<string> value)
            : base(name, ParameterAction.InfoDynamic)
        {
            ValuFunc = value;
        }

        public override string Value
        {
            get { return ValuFunc.Invoke(); }
            set { }
        }
    }
}
