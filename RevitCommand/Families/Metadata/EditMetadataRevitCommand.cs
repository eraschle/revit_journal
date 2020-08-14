using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using System;

namespace RevitCommand.Families.Metadata
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class EditMetadataRevitCommand : AFamilyRevitCommand<EditMetadataAction>
    {
        private const string debugLibraryPath = @"C:\develop\workspace\TEMP\JournalData\test files\";

        protected override Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
#if DEBUG
            commandData.JournalData.Add(Action.Library.JournalKey, debugLibraryPath);
#endif
            if (JournalKeyExist(commandData, Action.Library.JournalKey, out var library) == false)
            {
                message = "Journal Key for Library Path does not exist";
                return Result.Failed;
            }

            var revitFile = AFile.Create<RevitFamilyFile>(Document.PathName);
            var revitFamily = new RevitFamily(revitFile, library);
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
