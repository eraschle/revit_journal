using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAction.Action;
using RevitAction.Revit;

namespace RevitCommand.Families
{
    public abstract class AFamilyRevitCommand<TAction> : ARevitActionCommand<TAction> 
        where TAction : ITaskActionCommand, new()
    {
        protected Family Family { get; private set; }

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (Document.IsFamilyDocument == false)
            {
                message = "Is NOT a Revit Family file";
                TaskApp.Reporter.Error(message);
                return Result.Failed;
            }
            Family = Document.OwnerFamily;
            return InternalExecute(commandData, ref message, elements);
        }

        protected abstract Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements);
    }
}
