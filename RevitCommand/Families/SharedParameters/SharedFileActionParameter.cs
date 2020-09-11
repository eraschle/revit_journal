using DataSource.Model.SharedParameters;
using RevitAction.Action;
using System.Collections.Generic;

namespace RevitCommand.Families.SharedParameters
{
    public class SharedFileActionParameter : AActionParameter
    {
        public SharedFileActionParameter(string name) : base(name, "FilePath", ParameterKind.TextFile) { }

        public IList<SharedParameter> GetSharedParameters()
        {
            return SharedParameterReader.GetParameters(Value) ?? new List<SharedParameter>();
        }
    }
}
