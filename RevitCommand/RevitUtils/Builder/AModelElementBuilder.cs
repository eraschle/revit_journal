using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Create = Autodesk.Revit.Creation;
using RevitCommand.Repositories;

namespace RevitCommand.RevitUtils.Builder
{
    public abstract class AModelElementBuilder<TElement> : AModelBuilder<TElement> where TElement : Element
    {
        protected Create.FamilyItemFactory Factory { get; private set; }

        protected SubCategoryBuilder CategoryBuilder { get; private set; }

        protected AModelElementBuilder(Document document, Document source) : base(document)
        {
            Factory = Document.FamilyCreate;
            CategoryBuilder = new SubCategoryBuilder(Document, source);
        }

        public virtual IEnumerable<TElement> SourceElements(Document document)
        {
            return ElementRepo.GetElements<TElement>(document);
        }

        protected override TElement GetElement(ElementId elementId)
        {
            return ElementRepos.ById<TElement>(elementId);
        }

        protected override ElementId GetId(TElement source)
        {
            if (source is null) { return null; }

            return source.Id;
        }


        protected bool CopyParameter(TElement element, TElement created, IEnumerable<BuiltInParameter> bips)
        {
            if (bips is null) { return false; }

            var success = true;
            foreach (var parameter in bips)
            {
                success &= CopyParameter(element, created, parameter);
            }
            return success;
        }

        private bool CopyParameter(TElement element, TElement created, BuiltInParameter parameter)
        {
            if (element is null || created is null
                || parameter == BuiltInParameter.INVALID) { return false; }

            var original = element.get_Parameter(parameter);
            if (original is null || original.IsReadOnly) { return false; }

            var other = created.get_Parameter(parameter);
            switch (original.StorageType)
            {
                case StorageType.Integer:
                    return CopyInteger(original, other);
                case StorageType.Double:
                    return CopyDouble(original, other);
                case StorageType.String:
                    return CopyString(original, other);
                case StorageType.ElementId:
                    return CopyElementId(original, other);
                default:
                    return false;
            }
        }

        private bool CopyInteger(Parameter original, Parameter created)
        {
            var value = original.AsInteger();
            return value != 0 && created.Set(value);
        }

        private bool CopyDouble(Parameter original, Parameter created)
        {
            var value = original.AsDouble();
            return value != 0 && created.Set(value);
        }

        private bool CopyString(Parameter original, Parameter created)
        {
            var value = original.AsString();
            return created.Set(value);
        }

        private bool CopyElementId(Parameter original, Parameter created)
        {
            var value = original.AsElementId();
            return created.Set(value);
        }

        public override bool Equals(TElement element, TElement other)
        {
            return element != null && other != null
                && element.Id == other.Id
                && element.GetType() == other.GetType();
        }

        public override int GetHashCode(TElement obj)
        {
            int hashCode = -905324395;
            if (obj != null)
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<ElementId>.Default.GetHashCode(obj.Id);
                hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(obj.GetType());
            }
            return hashCode;
        }

    }
}
