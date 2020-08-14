using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitCommand.Families
{
    public abstract class AFamilyRevitCommand : ARevitExternalCommand
    {
        protected Family Family { get; private set; }

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (Document.IsFamilyDocument == false)
            {
                message = "Is NOT a Revit Family file";
                return Result.Failed;
            }
            Family = Document.OwnerFamily;
            return InternalExecute(commandData, ref message, elements);
        }

        protected abstract Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements);
    }
}
