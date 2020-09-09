using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAction.Action;

namespace RevitAction.Revit
{
    public class RevitExternalDialogCommand<TAction> : ARevitExternalCommand<TAction> where TAction : ITaskActionDialog, new()
    {
        protected RevitExternalDialogCommand() : base() { }

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UiApplication.PostCommand(RevitCommandId.LookupCommandId(Action.DialogId));
            return Result.Succeeded;
        }
    }
}
