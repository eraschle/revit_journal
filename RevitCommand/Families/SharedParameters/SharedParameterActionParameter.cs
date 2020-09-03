using RevitAction.Action;
using System;
using System.Collections.Generic;
using DataSource.Model.SharedParameters;

namespace RevitCommand.Families.SharedParameters
{
    public class SharedParameterActionParameter : AActionParameter
    {
        private readonly Func<IList<SharedParameter>> values;

        public SharedParameterActionParameter(string name, Func<IList<SharedParameter>> parmeterValues)
            : base(name, "Parameters", ParameterKind.List)
        {
            values = parmeterValues;
        }

        public IList<SharedParameter> ParameterValues
        {
            get { return values.Invoke(); }
        }
    }
}
