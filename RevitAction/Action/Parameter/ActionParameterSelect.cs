using System.Collections.Generic;

namespace RevitAction.Action.Parameter
{
    public class ActionParameterSelect : AActionParameter
    {
        public IList<string> Values { get; private set; }

        public ActionParameterSelect(string name, string journalKey, IList<string> values)
            : base(name, journalKey, ParameterKind.Selectable)
        {
            Values = values;
        }
    }
}
