using DataSource.Model.FileSystem;
using System;
using System.Xml.Linq;

namespace DataSource.Xml
{
    internal class RevitXmlRepository
    {
        private const string FamilyDataNode = "link";
        private const string CategoryNode = "category";
        private const string CategoryValueNode = "adsk:revit:grouping";
        private const string OmniClassNode = "std:oc1";
        private const string FamilyParameterParent = "features";
        private const string FamilyParameterNode = "group";
        private const string FamilyTypeParent = "family";
        private const string FamilyTypeNode = "part";
        private const string ParameterDisplayName = "displayName";

        private readonly FamilyXmlReader Reader;

        public RevitXmlRepository(RevitFamilyFile revitFile)
        {
            Reader = new FamilyXmlReader(revitFile);
        }

        internal void SetRevitFile(RevitFamilyFile revitFile)
        {
            if(revitFile is null) { return; }

            Reader.RevitFile = revitFile;
        }

        public void ReadMetaData()
        {
            Reader.ReadData();
        }

        public XElement OmniClass()
        {
            var omniClassNode = MoveToRootChild(CategoryNode);
            if (omniClassNode is null) { return null; }

            var value = First(omniClassNode);
            var schema = Next(value);
            if (Value(schema).Equals(OmniClassNode, StringComparison.CurrentCulture))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public XElement Category()
        {
            var categoryNode = MoveToRootChild(CategoryNode);
            if (categoryNode is null) { return null; }

            if (IsCategoryNode(categoryNode) == false)
            {
                categoryNode = MoveToNextByName(categoryNode, CategoryNode);
            }
            if (IsCategoryNode(categoryNode))
            {
                return First(categoryNode);
            }
            else
            {
                return null;
            }
        }

        private bool IsCategoryNode(XElement element)
        {
            var value = First(element);
            var schema = Next(value);
            return Value(schema).Equals(CategoryValueNode, StringComparison.CurrentCulture);
        }

        public XElement FamilyData()
        {
            var linkNode = MoveToRootChild(FamilyDataNode);
            if (linkNode is null) { return null; }

            var design = First(linkNode);
            return First(design);
        }

        public XElement FamilyParameters()
        {
            var features = MoveToRootChild(FamilyParameterParent);
            var feature = First(features);
            var title = First(feature);
            return MoveToNextByName(title, FamilyParameterNode);
        }

        public XElement FamilyParameterData(XElement element)
        {
            var title = First(element);
            return Next(title);
        }

        public string AttributeValue(XElement element, string name)
        {
            if (element is null) { return null; }

            var attribute = element.Attribute(name);
            if (attribute is null) { return string.Empty; }

            return attribute.Value;
        }

        public XElement FamilyTypesData()
        {
            var familyElement = MoveToRootChild(FamilyTypeParent);
            familyElement = First(familyElement);
            return MoveToNextByName(familyElement, FamilyTypeNode);
        }

        public XElement FamilyTypeName(XElement element)
        {
            if (element is null) { return null; }

            return First(element);
        }

        public string Value(XElement element)
        {
            if (element is null) { return string.Empty; }
            return element.Value;
        }

        public XElement Next(XElement element)
        {
            if (element is null) { return null; }

            var nextNode = element.NextNode;
            XElement nextElement = null;
            while (nextElement is null && nextNode != null)
            {
                nextElement = nextNode as XElement;
                nextNode = nextNode.NextNode;
            }
            return nextElement;
        }

        private XElement First(XElement element)
        {
            if (element is null) { return null; }

            var firstNode = element.FirstNode;
            XElement firstElement = null;
            while (firstElement is null && firstNode != null)
            {
                firstElement = firstNode as XElement;
                firstNode = firstNode.NextNode;
            }
            return firstElement;
        }

        private XElement MoveToRootChild(string name)
        {
            var rootChild = First(Reader.Root);
            return MoveToNextByName(rootChild, name);
        }

        private XElement MoveToNextByName(XElement element, string name)
        {
            if (element is null) { return null; }

            var nextElement = Next(element);
            while (nextElement != null && IsName(nextElement, name) == false)
            {
                nextElement = Next(nextElement);
            }
            return nextElement;
        }

        private bool IsName(XElement element, string name)
        {
            var elementName = GetLocalName(element);
            return element != null && elementName.Equals(name, StringComparison.CurrentCulture);
        }

        public string GetLocalName(XElement element)
        {
            if (element is null) { return string.Empty; }

            return element.Name.LocalName;
        }

        public string GetDisplayOrLocalName(XElement element)
        {
            if (element is null) { return string.Empty; }

            var displayName = AttributeValue(element, ParameterDisplayName);
            if (string.IsNullOrWhiteSpace(displayName) == false)
            {
                return displayName;
            }

            return GetLocalName(element);
        }
    }
}
