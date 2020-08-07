using DataSource.Model.Family;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DataSource.Xml
{
    internal class FamilyTypeBuilder
    {
        private RevitXmlRepository Repository;
        private IList<FamilyType> FamilyTypes;

        public IList<FamilyType> Build(RevitXmlRepository repository)
        {
            if (repository is null) { return null; }

            Repository = repository;
            FamilyTypes = new List<FamilyType>();

            var typeElement = Repository.FamilyTypesData();
            while (typeElement != null)
            {
                var type = Build(typeElement);
                FamilyTypes.Add(type);
                typeElement = Repository.Next(typeElement);

            }
            return FamilyTypes;
        }

        private FamilyType Build(XElement element)
        {
            if(element is null) { return null; }

            var nameNode = Repository.FamilyTypeName(element);
            var familyType = new FamilyType
            {
                Name = Repository.Value(nameNode)
            };
            BuildParameters(nameNode, familyType);
            return familyType;
        }

        private void BuildParameters(XElement element, FamilyType familyType)
        {
            var parameterNode = Repository.Next(element);
            while (parameterNode != null)
            {
                var parameter = ParameterBuilder.Build(Repository, parameterNode);
                familyType.AddParameter(parameter);
                parameterNode = Repository.Next(parameterNode);
            }
        }
    }
}
