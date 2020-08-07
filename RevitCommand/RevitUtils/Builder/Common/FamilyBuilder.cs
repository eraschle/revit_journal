using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using RevitCommand.Repositories;
using System.Collections.Generic;

namespace RevitCommand.RevitUtils.Builder
{
    public class FamilyBuilder : AModelElementBuilder<FamilyInstance>, IEqualityComparer<FamilySymbol>
    {
        public FamilyBuilder(Document document, Document source) : base(document, source) { }

        protected override FamilyInstance CreateElement(FamilyInstance source)
        {
            if (source is null) { return null; }

            var reference = ReferenceRepo.Get(source);
            var host = ElementRepos.GetCreated(reference);
            var location = (source.Location as LocationPoint).Point;
            var symbol = GetSymbol(source.Symbol);
            var newFamily = Factory.NewFamilyInstance(location, symbol, host, StructuralType.NonStructural);
            return newFamily;
        }

        protected override void MergeElement(FamilyInstance source, ref FamilyInstance created) { }

        protected override void AddCreatedElements(FamilyInstance source, FamilyInstance created)
        {
            BuildManager.Add(source, created);
            var sourceRef = ReferenceRepo.Get(source);
            if(ElementRepos.IsCreated(sourceRef)) { return; }

            var createdRef = ReferenceRepo.Get(created);
            if (sourceRef is null || createdRef is null) { return; }

            BuildManager.Add(sourceRef.ElementId, createdRef.ElementId);
        }

        public override IEnumerable<FamilyInstance> SourceElements(Document document)
        {
            var elements = base.SourceElements(document);

            return elements;
        }


        private FamilySymbol GetSymbol(FamilySymbol sourceSymbol)
        {
            foreach (var symbol in ElementRepos.GetElements<FamilySymbol>())
            {
                if (Equals(symbol, sourceSymbol) == false) { continue; }

                if (symbol.IsActive == false)
                {
                    symbol.Activate();
                }
                return symbol;
            }
            return null;
        }


        public override bool Equals(FamilyInstance element, FamilyInstance other)
        {
            return base.Equals(element, other)
                && element != null && other != null
                && Equals(element.Symbol, other.Symbol);
        }

        public override int GetHashCode(FamilyInstance obj)
        {
            int hashCode = base.GetHashCode(obj);
            if (obj != null)
            {
                hashCode = hashCode * -1521134295 + GetHashCode(obj.Symbol);
            }
            return hashCode;
        }

        public bool Equals(FamilySymbol symbol, FamilySymbol other)
        {
            return symbol != null && other != null
                && symbol.FamilyName == other.FamilyName
                && symbol.Name == other.Name;
        }

        public int GetHashCode(FamilySymbol obj)
        {
            int hashCode = -1380918663;
            if (obj != null)
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.FamilyName);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Name);
            }
            return hashCode;
        }
    }
}
