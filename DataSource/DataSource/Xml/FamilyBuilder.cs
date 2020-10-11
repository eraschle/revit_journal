using DataSource.Helper;
using DataSource.Models.Catalog;
using DataSource.Models.Product;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Utilities.System;
using DataSource.Model.Metadata;

namespace DataSource.Xml
{
    internal class FamilyBuilder
    {
        private Family Family;
        private RevitXmlRepository Repository;

        public Family Build(RevitXmlRepository repository)
        {
            if (repository is null) { return null; }

            Repository = repository;
            Family = new Family
            {
                Source = Source.File,
                SourceUpdated = DateTime.Now
            };

            BuildCommonData();
            BuildCommonParameters();
            Family.DisplayName = ModelHelper.FamilyDisplayName(Family);
            return Family;
        }

        private void BuildCommonData()
        {
            var node = Repository.Category();
            var category = Repository.Value(node);
            if (string.IsNullOrWhiteSpace(category) == false)
            {
                Family.Category = new Category { Name = category };
            }

            var familyData = Repository.FamilyData();
            Family.Name = Repository.Value(familyData);

            familyData = Repository.Next(familyData);
            var productName = Repository.Value(familyData);

            familyData = Repository.Next(familyData);
            var productVersion = Repository.Value(familyData);
            if (int.TryParse(productVersion, out var version))
            {
                const bool executable = false;
                RevitApp product;
                if (ProductManager.HasVersion(version, executable))
                {
                    product = ProductManager.GetVersion(version, executable);
                }
                else
                {
                    product = ProductManager.CreateCustom(productName, version);
                    ProductManager.AddCustom(product);
                }
                Family.Product = product;
            }

            familyData = Repository.Next(familyData);
            var updated = Repository.Value(familyData);
            Family.Updated = DateUtils.AsDate(updated);
        }

        private void BuildCommonParameters()
        {
            var group = Repository.FamilyParameters();
            while (group != null)
            {
                BuildGroupParameter(group);
                group = Repository.Next(group);
            }
        }

        private void BuildGroupParameter(XElement group)
        {
            var paramElement = Repository.FamilyParameterData(group);
            var omniClass = new List<Parameter>();
            while (paramElement != null)
            {
                var parameter = ParameterBuilder.Build(Repository, paramElement);
                if (parameter.Name.Equals(Family.OmniClassNumber, StringComparison.CurrentCulture)
                    || parameter.Name.Equals(Family.OmniClassText, StringComparison.CurrentCulture))
                {
                    omniClass.Add(parameter);
                }
                else
                {
                    Family.AddParameter(parameter);
                }
                paramElement = Repository.Next(paramElement);
            }
            if (omniClass.Count == 2)
            {
                string number, name;
                if (omniClass[0].Name.Equals(Family.OmniClassNumber, StringComparison.CurrentCulture))
                {
                    number = omniClass[0].Value;
                    name = omniClass[1].Value;
                }
                else
                {
                    number = omniClass[1].Value;
                    name = omniClass[0].Value;
                }
                if (string.IsNullOrWhiteSpace(number) == false
                    && string.IsNullOrWhiteSpace(name) == false)
                {
                    Family.OmniClass = new OmniClass
                    {
                        IdArray = OmniClass.GetId(number),
                        Name = name
                    }; ;
                }
            }
        }
    }
}
