using RevitJournal.Journal.Command;
using RevitJournal.Revit.SharedParameters;
using System;
using System.Collections.Generic;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class SharedParameterCommandParameter : CommandParameterExternal
    {
        private const string DefaultParameterName = "Parameters";

        private readonly Func<IList<SharedParameter>> ParmeterValuesFunc;
        public SharedParameterCommandParameter(string jounralKey, Func<IList<SharedParameter>> parmeterValues, string parameterName = DefaultParameterName)
            : base(jounralKey, parameterName, JournalParameterType.List, true)
        {
            ParmeterValuesFunc = parmeterValues;
        }

        public IList<SharedParameter> ParameterValues { get { return ParmeterValuesFunc.Invoke(); } }
    }
}
