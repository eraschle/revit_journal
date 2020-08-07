using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using RevitCommand.RevitUtils.Builder;
using RevitCommand.RevitUtils.Builder.Family;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace RevitCommand.Families.Translation
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class TranslateFamilyCommand : AFamilyExternalCommand
    {
        private bool HasOther(out Document document)
        {
            document = null;
            foreach (Document doc in Application.Documents)
            {
                if (doc.IsFamilyDocument == false || Document.Equals(doc)) { continue; }

                document = doc;
                break;
            }
            return document != null;
        }

        protected override Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (HasOther(out var otherDocument) == false)
            {
                return Result.Cancelled;
            }

            using (var group = new TransactionGroup(otherDocument, "Create"))
            {
                group.Start();
                try
                {
                    //CopyElementSolution(otherDocument);
                    CopyElement(otherDocument);
                    group.Assimilate();
                }
                catch (Exception)
                {
                    group.RollBack();
                    throw;
                }
            }

            return Result.Succeeded;
        }

        #region self implemented copy version

        private void CopyElement(Document otherDocument)
        {
            using (var transaction = new Transaction(otherDocument, "Copy Elements"))
            {
                transaction.Start();
                try
                {
                    CreateCopy(new DatumPlaneBuilder(otherDocument, Document));
                    CreateCopy(new ExtrusionBuilder(otherDocument, Document));
                    CreateCopy(new BlendBuilder(otherDocument, Document));
                    CreateCopy(new FamilyBuilder(otherDocument, Document));
                    CreateCopy(new DimensionBuilder(otherDocument, Document));
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                }
            }
        }

        private void CreateCopy<TElement>(AModelElementBuilder<TElement> builder) where TElement : Element
        {
            foreach (var element in builder.SourceElements(Document))
            {
                builder.CreateTransaction(element);
            }
        }

        #endregion

        #region CopyElements with API

        private ICollection<ElementId> CopyElementSolution(Document otherDocument)
        {
            ICollection<ElementId> copiedElements = null;
            using (var transaction = new Transaction(otherDocument, "Copy Elements"))
            {
                transaction.Start();
                try
                {
                    var option = new CopyPasteOptions();
                    var elementFilter = GetElementFilter();
                    var sourceIds = GetElements(elementFilter);
                    copiedElements = ElementTransformUtils.CopyElements(Document, sourceIds, otherDocument, null, option);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                    throw;
                }
            }
            return copiedElements;
        }

        private ICollection<ElementId> GetElements(ElementFilter filter)
        {
            var col = new FilteredElementCollector(Document);
            col = col.WherePasses(filter);
            return col.ToElementIds();
        }

        private ElementFilter GetElementFilter()
        {
            var elementTypes = new List<Type>
            {
                typeof(GenericForm),
                typeof(Family),
                typeof(FamilySymbol),
                typeof(FamilyInstance),
                typeof(Group),
                typeof(GroupType),
                typeof(ConnectorElement),
            };
            return new ElementMulticlassFilter(elementTypes);
        }

        #endregion
    }
}
