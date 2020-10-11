using RevitAction.Action;
using System;
using System.Collections.Generic;
using DataSource.Models.SharedParameters;
using RevitAction.Action.Parameter;

namespace RevitCommand.Families.SharedParameters
{
    public class SharedParameterActionParameter : AActionParameter
    {
        private const string ParametersKey = "Parameters";

        public Func<IList<SharedParameter>> SharedValuesFunc { get; set; }

        public SharedParameterActionParameter(string name, Func<IList<SharedParameter>> parmeterValues) : base(name, ParametersKey, ParameterKind.List)
        {
            SharedValuesFunc = parmeterValues;
        }

        public IList<SharedParameter> SharedValues
        {
            get { return SharedValuesFunc.Invoke(); }
        }

        public List<string> ParameterNames { get; } = new List<string>();

        public override string GetJournalValue()
        {
            return ParameterListConverter.GetLine(ParameterNames);
        }

        public override void SetJournalValue(string journalValue)
        {
            ParameterNames.Clear();
            ParameterNames.AddRange(ParameterListConverter.GetList(journalValue));
        }
    }
}
