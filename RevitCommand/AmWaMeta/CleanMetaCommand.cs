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
                TaskApp.Reporter.WarningReport($"No schema {schemaGuid}");
                return Result.Succeeded;
            }

            using (Transaction transaction = new Transaction(Document))
            {
                transaction.Start("Extensible storage deleted");
                try
                {
                    Schema.EraseSchemaAndAllEntities(schema, true);
                    transaction.Commit();
                    TaskApp.Reporter.CustomReport(transaction.GetName());
                }
                catch (Exception exception)
                {
                    transaction.RollBack();
                    throw new Exception(transaction.GetName(), exception);
                }
            }
            Document.Save();
            TaskApp.Reporter.CustomReport($"Saved changes to {Document.PathName}");
            return Result.Succeeded;
        }
    }
}
