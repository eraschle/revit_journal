using RevitAction.Revit;
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.Attributes;

namespace RevitCommand.AmWaMeta
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class CleanMetaCommand : ARevitActionCommand<CleanMetaAction>
    {
        public static readonly Guid schemaGuid = new Guid("{61EB3D3C-F5DE-4FCA-8A7C-0105122C62BB}");

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Schema schema = Schema.Lookup(schemaGuid);
            if (schema == null)
            {
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.StatusReport($"No schema {schemaGuid}");
                return Result.Succeeded;
            }

            var makesChanges = false;
            using (Transaction transaction = new Transaction(Document))
            {
                try
                {
                    transaction.Start("Extensible storage deleted");
                    Schema.EraseSchemaAndAllEntities(schema, true);
                    transaction.Commit();
                    makesChanges = true;
                    TaskApp.Reporter.ActionId = Action.Id;
                    TaskApp.Reporter.StatusReport(transaction.GetName());
                }
                catch (Exception ex)
                {
                    makesChanges = false;
                    TaskApp.Reporter.ActionId = Action.Id;
                    TaskApp.Reporter.Error(transaction.GetName(), ex);
                    transaction.RollBack();
                }
            }
            if (makesChanges)
            {
                Document.Save();
            }
            return Result.Succeeded;
        }
    }
}
