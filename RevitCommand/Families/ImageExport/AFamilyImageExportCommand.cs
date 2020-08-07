using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitCommand.Families.ImageExport
{
    public abstract class AFamilyImageExportCommand : AFamilyExternalCommand
    {
        protected ImageExportManager Manager;
        protected readonly ElementFilter ExcludeHeads;

        protected readonly ElementFilter NoCameraFilter
            = new ElementCategoryFilter(BuiltInCategory.OST_Cameras, true);

        protected readonly ElementFilter NoDimensionFilter
            = new ElementCategoryFilter(BuiltInCategory.OST_Dimensions, true);

        protected readonly ElementFilter FamilySymbolFilter
            = new ElementClassFilter(typeof(FamilySymbol));

        protected readonly ElementFilter NotViewFilter
            = new ElementCategoryFilter(BuiltInCategory.OST_Views, true);


        protected AFamilyImageExportCommand() : base()
        {
            var categories = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_LevelHeads,
                BuiltInCategory.OST_SectionHeads,
                BuiltInCategory.OST_GridHeads
            };
            ExcludeHeads = new ElementMulticategoryFilter(categories, true);
        }

        protected override Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Manager = new ImageExportManager(UIDocument);
            using (var transactionGroup = new TransactionGroup(Document, "ExportImage"))
            {
                transactionGroup.Start();
                try
                {
                    return ExportImage(commandData, ref message, elements);
                }
                catch (Exception exp)
                {
                    message = exp.Message;
                    return Result.Failed;
                }
                finally
                {
                    transactionGroup.RollBack();
                }
            }
        }

        protected bool HasFamilySymbols(out FilteredElementCollector collector)
        {
            collector = new FilteredElementCollector(Document)
                .WherePasses(FamilySymbolFilter);
            return collector.GetElementCount() > 0;
        }

        protected bool HasFamilyInstancesOfSymbol(FamilySymbol familySymbol, bool onlyVisible, out FilteredElementCollector collector)
        {
            if (onlyVisible)
            {
                collector = GetViewCollector();
            }
            else
            {
                collector = new FilteredElementCollector(Document);
            }
            var filter = new FamilyInstanceFilter(Document, familySymbol.Id);
            return collector.WherePasses(filter).GetElementCount() > 0;
        }

        protected bool HasFamilyInstances(out FilteredElementCollector collector)
        {
            collector = null;
            if (HasFamilySymbols(out var symbolCollector) == false) { return false; }

            var symbolFilters = new List<ElementId>();
            foreach (var element in symbolCollector)
            {
                if (!(element is FamilySymbol familySymbol)
                    || HasFamilyInstancesOfSymbol(familySymbol, true, out var instanceCollector) == false) { continue; }

                symbolFilters.AddRange(instanceCollector.ToElementIds());
            }
            if (symbolFilters.Count > 0)
            {
                collector = new FilteredElementCollector(Document, symbolFilters);
            }
            return collector != null && collector.GetElementCount() > 0;
        }

        protected bool HasGroups(out FilteredElementCollector collector)
        {
            collector = new FilteredElementCollector(Document).OfClass(typeof(Group));
            return collector.GetElementCount() > 0;
        }

        protected static ICollection<ElementId> GetGroupElementIds(Element element)
        {
            var elementIds = new List<ElementId>
            {
                element.Id
            };
            if (!(element is Group group)) { return elementIds; }

            elementIds.AddRange(group.GetMemberIds());
            return elementIds;
        }

        protected bool HasModelText(out FilteredElementCollector collector)
        {
            collector = GetViewCollector().OfClass(typeof(ModelText));
            return collector.GetElementCount() > 0;
        }

        protected static bool HaveModelTextSameLocation(FilteredElementCollector collector)
        {
            bool similar = false;
            LocationPoint location = null;
            foreach (var element in collector.ToElements())
            {
                if (!(element is ModelText modelText)
                    || !(modelText.Location is LocationPoint point)) { continue; }

                if (location is null)
                {
                    location = point;
                }
                else
                {
                    similar |= location.Point.IsAlmostEqualTo(point.Point);
                }
            }
            return similar;
        }

        protected bool HasDetailElements(out FilteredElementCollector collector)
        {
            collector = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_DetailComponents);
            return collector.GetElementCount() > 0;
        }

        protected FilteredElementCollector GetViewCollector()
        {
            return new FilteredElementCollector(Document, Document.ActiveView.Id)
                .WherePasses(NoCameraFilter);
        }

        protected void HideOtherElements(FilteredElementCollector collector)
        {
            if (collector is null) { return; }

            collector.WherePasses(NoDimensionFilter);
            var toHide = new FilteredElementCollector(Document).WherePasses(NotViewFilter);
            if (collector.GetElementCount() > 0)
            {
                toHide.Excluding(collector.ToElementIds());
            }
            HideElements(toHide);
        }

        protected bool AreAllElementsHidden(out FilteredElementCollector visibleCollector)
        {
            visibleCollector = GetViewCollector().WherePasses(NoCameraFilter);
            return visibleCollector.GetElementCount() == 0;
        }

        protected void UnhideElements(FilteredElementCollector collector)
        {
            if (collector is null || collector.GetElementCount() == 0) { return; }

            Unhide(collector.ToElementIds());
        }

        protected void Unhide(ICollection<ElementId> elementIds)
        {
            if (elementIds.Count == 0) { return; }

            using (var transaction = new Transaction(Document, "UnhideElements"))
            {
                transaction.Start();
                Document.ActiveView.UnhideElements(elementIds);
                transaction.Commit();
            }
        }

        protected void HideElements(FilteredElementCollector collector)
        {
            if (collector is null || collector.GetElementCount() == 0) { return; }

            Hide(collector.ToElements());
        }

        protected void HideElement(Element element)
        {
            if (element is null) { return; }

            Hide(new List<Element> { element });
        }

        protected void Hide(ICollection<Element> elements)
        {
            if (elements.Count == 0) { return; }

            var view = Document.ActiveView;
            var elementIds = elements
              .Where(ele => ele.CanBeHidden(view))
              .Select(ele => ele.Id);

            if (elementIds.Any() == false) { return; }

            using (var transaction = new Transaction(Document, "HideElements"))
            {
                transaction.Start();
                view.HideElements(elementIds.ToList());
                transaction.Commit();
            }
        }

        protected void SetLineColorAndWeight(Color color, int lineWeight)
        {
            if (AreAllElementsHidden(out var elements)) { return; }

            SetCategoryDesign(Family.FamilyCategory, color, lineWeight);
            foreach (var element in elements)
            {
                var category = element.Category;
                if (category is null) { continue; }

                SetCategoryDesign(category, color, lineWeight);
            }
        }

        private void SetCategoryDesign(Category category, Color color, int lineWeight)
        {
            if (category is null) { return; }

            category.LineColor = color;
            category.SetLineWeight(lineWeight, GraphicsStyleType.Projection);
            var linePattern = LinePatternElement.GetSolidPatternId();
            if (linePattern != null)
            {
                category.SetLinePatternId(linePattern, GraphicsStyleType.Projection);
            }
            if (category.CanAddSubcategory)
            {
                foreach (Category subCategory in category.SubCategories)
                {
                    SetCategoryDesign(subCategory, color, lineWeight);
                }
            }
        }

        private View GetView(Predicate<View> isCorrectView)
        {
            View correctView = null;
            var collector = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Views);
            foreach (var element in collector.ToElements())
            {
                if (!(element is View view) || isCorrectView(view) == false) { continue; }

                correctView = element as View;
                break;
            }
            return correctView;
        }

        protected bool SetCorrectView(Predicate<View> isCorrectView)
        {
            var correctView = GetView(isCorrectView);
            if (correctView is null) { return false; }

            SetActiveView(correctView);
            return true;
        }

        protected void SetActiveView(View view)
        {
            if (Document.ActiveView.Id != view.Id)
            {
                UIDocument.ActiveView = view;
            }
        }

        protected abstract Result ExportImage(ExternalCommandData commandData, ref string message, ElementSet elements);
    }
}
