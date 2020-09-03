using DataSource.Model.Family;
using System.Xml.Linq;

namespace DataSource.Xml
{
    internal static class ParameterBuilder
    {
        private const string ParameterId = "id";
        private const string ParametervalueType = "typeOfParameter";
        private const string ParameterType = "type";

        private const string GermanYes = "Ja";
        private const string GermanNo = "Nein";

        public static Parameter Build(RevitXmlRepository repository, XElement element)
        {
            if (repository is null) { return null; }

            var value = repository.Value(element);
            if (value.Equals(GermanYes)) { value = bool.TrueString; }
            if (value.Equals(GermanNo)) { value = bool.FalseString; }

            var parameter = new Parameter
            {
                Id = repository.AttributeValue(element, ParameterId),
                Name = repository.GetDisplayOrLocalName(element),
                ValueType = repository.AttributeValue(element, ParametervalueType),
                Value = value,
                ParameterType = repository.AttributeValue(element, ParameterType),
            };
            return parameter;
        }
    }
}
