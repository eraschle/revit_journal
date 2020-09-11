﻿using System.Collections.Generic;

namespace RevitAction.Action.Parameter
{
    public class ActionParameterSelect : AActionParameter
    {
        public IList<string> SelectableValues { get; private set; }

        public ActionParameterSelect(string name, string journalKey, IList<string> selectableValues)
            : base(name, journalKey, ParameterKind.Selectable)
        {
            SelectableValues = selectableValues;
        }
    }
}
