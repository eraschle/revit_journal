using Autodesk.Revit.DB;
using RevitCommand.RevitUtils.Builder;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitCommand.Repositories
{
    public class ElementRepo
    {
        public static IEnumerable<ElementId> GetIds(IEnumerable<Element> elements)
        {
            return elements.Select(ele => ele.Id);
        }

        public static IList<ElementId> GetListIds(IEnumerable<Element> elements)
        {
            return GetIds(elements).ToList();
        }

        public static IEnumerable<TElement> GetElements<TElement>(Document document) where TElement : Element
        {
            if (document is null) { return null; }

            using (var collector = new FilteredElementCollector(document))
            {
                return collector.OfClass(typeof(TElement))
                                .ToElements()
                                .OfType<TElement>();
            }
        }

        public static TElement ById<TElement>(Document document, ElementId elementId) where TElement : Element
        {
            if (document is null || elementId is null) { return null; }

            return document.GetElement(elementId) as TElement;
        }

        public static bool HasByName<TElement>(Document document, string name, out TElement element) where TElement : Element
        {
            element = ByName<TElement>(document, name);
            return element != null;
        }

        public static TElement ByName<TElement>(Document document, string name) where TElement : Element
        {
            foreach (var element in GetElements<TElement>(document))
            {
                if (IsName(element, name) == false) { continue; }

                return element;
            }
            return null;
        }

        public static bool IsName(Element element, string name)
        {
            return string.IsNullOrWhiteSpace(name) == false
                && element != null
                && string.IsNullOrWhiteSpace(element.Name) == false
                && element.Name.Equals(name, StringComparison.CurrentCulture);
        }

        private readonly Document Document;

        public ElementRepo(Document document)
        {
            Document = document;
        }

        public IEnumerable<TElement> GetElements<TElement>() where TElement : Element
        {
            return GetElements<TElement>(Document);
        }

        public bool HasById(ElementId elementId, out Element element)
        {
            element = ById<Element>(elementId);
            return element != null;
        }

        public bool HasById(ElementId elementId)
        {
            return ById<Element>(elementId) != null;
        }

        public TElement ById<TElement>(ElementId elementId) where TElement : Element
        {
            return ById<TElement>(Document, elementId);
        }

        public bool HasByName<TElement>(string name, out TElement element) where TElement : Element
        {
            return HasByName(Document, name, out element);
        }

        public TElement ByName<TElement>(string name) where TElement : Element
        {
            return ByName<TElement>(Document, name);
        }

        public bool IsCreated<TElement>(ElementId elementId, out TElement element) where TElement : Element
        {
            element = null;
            if (BuildManager.IsCreated(elementId, out elementId))
            {
                element = ById<TElement>(elementId);
            }
            return element != null;
        }

        public Element GetCreated(Reference reference)
        {
            if (BuildManager.IsCreated(reference.ElementId, out var elementId) == false)
            {
                elementId = reference.ElementId;
            }
            return ById<Element>(elementId);
        }

        public bool IsCreated(Reference reference, out Element element)
        {
            element = GetCreated(reference);
            return element != null;
        }

        public bool IsCreated(Reference reference)
        {
            return GetCreated(reference) != null;
        }
    }
}
