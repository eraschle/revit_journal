using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitAction.Revit;
using System;

namespace RevitCommand.Families.Metadata
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class EditMetadataCommand : ARevitActionCommand<EditMetadataAction>
    {
        private const string debugLibraryPath = @"C:\develop\workspace\TEMP\JournalData\test files\";

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
#if DEBUG
            Action.Library.Value = debugLibraryPath;
#endif
            var revitFile = AFile.Create<RevitFamilyFile>(Document.PathName);
            var revitFamily = new RevitFamily(revitFile, Action.Library.Value);
            var metaFamily = revitFamily.ReadEditedMetaData();

            var manager = new RevitMetadataManager(Document);

            var updater = new RevitFamilyParameterUpdater(Document);
            var editedFamily = manager.CreateFamily();
            using (var transactionGroup = new TransactionGroup(Document, "Edit Metadata"))
            {
                transactionGroup.Start();
                try
                {
                    updater.UpdateMetadata(metaFamily, editedFamily);
                    transactionGroup.Commit();
                }
                catch (Exception)
                {
                    transactionGroup.RollBack();
                }
            }

            return Result.Succeeded;
        }
    }
}
