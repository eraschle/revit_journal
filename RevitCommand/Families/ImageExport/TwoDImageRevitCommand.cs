using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace RevitCommand.Families.ImageExport
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class TwoDImageRevitCommand : AFamilyImageExportCommand<TwoDImageAction>
    {
        public const string ClassName = "TwoDImageExportExternalCommand";
        private const string ImageSuffix = "Symbol";

        private readonly ElementFilter SymbolFilter;

        public TwoDImageRevitCommand() : base()
        {
            var symbolFilter = new ElementCategoryFilter(BuiltInCategory.OST_DetailComponents);
            var notSymbolFilter = new ElementCategoryFilter(BuiltInCategory.OST_Dimensions, true);
            SymbolFilter = new LogicalAndFilter(symbolFilter, notSymbolFilter);
        }

        protected override Result ExportImage(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (SetCorrectView(IsCorrectView) == false)
            {
                return Result.Succeeded;
            }

            SetupView();

            if (AreAllElementsHidden(out var visible))
            {
                foreach (var view in GetElevationViews())
                {
                    SetActiveView(view);
                    SetupView();
                    if (AreAllElementsHidden(out visible) == false)
                    {
                        break;
                    }
                }
            }

            if (AreAllElementsHidden(out visible))
            {
                Manager.CreateNoSymbolImage(ImageSuffix);
                return Result.Succeeded;
            }

            using (var transaction = new Transaction(Document, "SetupView"))
            {
                transaction.Start();
                Document.ActiveView.Scale = 2;
                SetLineColorAndWeight(new Color(0, 0, 0), 1);
                transaction.Commit();
            }
            var options = Manager.DefaultOptions(ImageSuffix);
            Manager.ExportImage(options);
            return Result.Succeeded;
        }

        private void SetupView()
        {
            var collector = GetViewCollector().WherePasses(SymbolFilter);
            if (HasFamilyInstances(out var familyInstance))
            {
                if (collector.GetElementCount() == 0)
                {
                    collector = familyInstance;
                }
                else
                {
                    collector.IntersectWith(familyInstance);
                }
            }
            collector.WherePasses(ExcludeHeads);
            HideOtherElements(collector);
            if (AreAllElementsHidden(out _)
                && HasDetailElements(out var detailCollector))
            {
                detailCollector.WhereElementIsElementType();
                UnhideElements(detailCollector);
            }
            //TODO Unsure if the corrent implementation is correct
            //else if (AreAllElementsHidden(out visible) == false
            //    && HasDetailElements(out detailCollector))
            //{
            //    detailCollector.WhereElementIsElementType();
            //    var firstId = detailCollector.FirstElementId();
            //    Unhide(new List<ElementId> { firstId });
            //}
        }

        private IEnumerable<View> GetElevationViews()
        {
            var sectionViews = new List<View>();
            var collector = new FilteredElementCollector(Document).OfClass(typeof(View));
            foreach (var element in collector.ToElements())
            {
                if (!(element is View view) || view.ViewType != ViewType.Elevation) { continue; }

                sectionViews.Add(view);
            }
            return sectionViews;
        }

        public bool IsCorrectView(View view)
        {
            if (view.ViewType != ViewType.FloorPlan || view.IsTemplate) { return false; }

            var viewFamilyType = Document.GetElement(view.GetTypeId());
            return viewFamilyType is ViewFamilyType viewFamily && viewFamily.PlanViewDirection == PlanViewDirection.Down;
        }
    }
}
