using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RevitCommand.Families.ImageExport
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class GeomCombinationCommand : IExternalCommand
    {
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var uiDoc = commandData.Application.ActiveUIDocument;
                var selection = uiDoc.Selection;
                var reference = selection.PickObject(ObjectType.Element, new SelectionFilter());
                var genericForm = uiDoc.Document.GetElement(reference.ElementId) as GenericForm;
                var allElements = GetElements(new List<Element>(), genericForm.Combinations);
                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }

        private List<Element> GetElements(List<Element> allElements, GeomCombinationSet combinationSet)
        {
            foreach (GeomCombination geoCombination in combinationSet)
            {
                foreach (CombinableElement combinationElement in geoCombination.AllMembers)
                {
                    if(allElements.Contains(combinationElement)) { continue; }

                    allElements.Add(combinationElement);
                    allElements = GetElements(allElements, combinationElement.Combinations);
                }
            }
            return allElements;
        }
    }

    public class SelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            return elem is GenericForm;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
