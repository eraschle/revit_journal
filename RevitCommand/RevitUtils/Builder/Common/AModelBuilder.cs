using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using RevitCommand.Repositories;

namespace RevitCommand.RevitUtils.Builder
{
    public abstract class AModelBuilder<TElement> : IEqualityComparer<TElement> where TElement : class
    {
        protected Document Document { get; private set; }

        protected Application Application { get; private set; }

        protected View GetView(ElementId activeSource)
        {
            return Document.GetElement(activeSource) as View;
        }

        protected ElementRepo ElementRepos { get; private set; }

        protected AModelBuilder(Document document)
        {
            if (document is null) { throw new ArgumentNullException(nameof(document)); }

            Document = document;
            Application = document.Application;
            ElementRepos = new ElementRepo(Document);
        }

        public TElement Create(TElement source)
        {
            if (source is null) { return null; }

            var sourceId = GetId(source);
            if (BuildManager.IsCreated(sourceId, out var elementId))
            {
                return GetElement(elementId);
            }

            var element = GetElement(sourceId);
            if (Equals(source, element) == false)
            {
                element = CreateElement(source);
            }
            if (element is null) { return null; }

            MergeElement(source, ref element);
            AddCreatedElements(source, element);
            BuildManager.Add(GetId(source), GetId(element));
            return element;
        }

        protected abstract TElement CreateElement(TElement source);

        protected abstract void MergeElement(TElement source, ref TElement created);

        protected abstract void AddCreatedElements(TElement source, TElement created);

        protected abstract ElementId GetId(TElement source);

        protected abstract TElement GetElement(ElementId elementId);

        public TElement CreateTransaction(TElement source)
        {
            using (var transaction = new Transaction(Document, $"Create {nameof(TElement)}"))
            {
                transaction.Start();
                try
                {
                    var created = Create(source);
                    transaction.Commit();
                    return created;
                }
                catch (Exception)
                {
                    transaction.RollBack();
                    throw;
                }
            }
        }

        public abstract bool Equals(TElement element, TElement other);
        public abstract int GetHashCode(TElement obj);
    }
}
