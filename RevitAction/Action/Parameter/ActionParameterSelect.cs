using System.Collections.Generic;

namespace RevitAction.Action.Parameter
{
    public class ActionParameterSelect : AActionParameter
    {
        public IList<string> SelectableValues { get; private set; }

        public ActionParameterSelect(string name, IList<string> selectableValues)
            : base(name, "Selected", ParameterKind.Selectable)
        {
            SelectableValues = selectableValues;
        }
    }
}
