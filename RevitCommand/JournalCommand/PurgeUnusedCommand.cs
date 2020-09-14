using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAction.Action;
using RevitAction.Revit;
using System;
using System.Collections.Generic;

namespace RevitCommand.JournalCommand
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class PurgeUnusedCommand : ARevitActionCommand<PurgeUnusedAction>
    {
        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using (Transaction transaction = new Transaction(Document))
            {
                transaction.Start("Purge unused Elements");
                try
                {
                    var commandId = RevitCommandId.LookupCommandId(Action.RevitCommand.Value);
                    var count = 0;
                    while (count < Action.Repetitions.GetIntValue())
                    {
                        UiApplication.PostCommand(commandId);
                    }
                    transaction.Commit();
                    TaskApp.Reporter.CustomReport(transaction.GetName());
                }
                catch (Exception exception)
                {
                    transaction.RollBack();
                    throw new Exception(transaction.GetName(), exception);
                }
            }
            return Result.Succeeded;
        }
    }
}
