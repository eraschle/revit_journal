using System.Collections.Generic;
using DataSource.Model.Family;

namespace RevitCommand.Families.Metadata
{
    public class MetadataParameterComparer : IEqualityComparer<Parameter>
    {
        public bool Equals(Parameter parameter, Parameter other)
        {
            return parameter != null && other != null 
                && parameter.Value == other.Value;
        }

        public int GetHashCode(Parameter obj)
        {
            return EqualityComparer<string>.Default.GetHashCode(obj.Value);
        }
    }
}
