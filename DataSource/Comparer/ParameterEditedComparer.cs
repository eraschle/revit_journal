using DataSource.Model.Metadata;
using System.Collections.Generic;

namespace DataSource.Comparer
{
    public class ParameterEditedComparer : IEqualityComparer<Parameter>
    {
        public bool ParametersEquals(IList<Parameter> parameters, IList<Parameter> other)
        {
            var equals = parameters != null && other != null 
                           && parameters.Count == other.Count;
            if (equals == false) { return false; }

            foreach (var parameter in parameters)
            {
                equals &= other.Contains(parameter);
                if (equals == false) { break; }

                var idx = other.IndexOf(parameter);
                equals &= Equals(parameter, other[idx]);
                if (equals == false) { break; }
            }
            return equals;
        }

        public bool Equals(Parameter parameter, Parameter other)
        {
            var parameterEquals = parameter != null && other != null
                && parameter.Id == other.Id
                && parameter.Name == other.Name
                && parameter.Value == other.Value;
            return parameterEquals;
        }

        public int GetHashCode(Parameter obj)
        {
            var hashCode = -691830078;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Value);
            return hashCode;
        }
    }
}
