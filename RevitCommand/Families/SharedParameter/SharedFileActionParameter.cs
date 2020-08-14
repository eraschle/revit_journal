using DataSource.Model.SharedParameters;
using RevitAction.Action;
using System.Collections.Generic;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class SharedFileActionParameter : AActionParameter
    {
        public SharedFileActionParameter(string name)
            : base(name, "FilePath", ParameterKind.TextFile) { }

        public IList<SharedParameter> GetSharedParameters()
        {
            var names = SharedParameterReader.GetParameters(Value);
            if (names is null)
            {
                names = new List<SharedParameter>();
            }
            return names;
        }
    }
}
