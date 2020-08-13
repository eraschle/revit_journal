using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.DataSource.Json;
using DataSource.Helper;
using DataSource.Json;
using DataSource.Model.FileSystem;
using System;
using DS = DataSource.Model;

namespace RevitCommand.Families.Metadata
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class EditMetadataExternalCommand : AFamilyExternalCommand
    {
        private const string TransactionName = "Edit Metadata";
        private const string DebugLibraryPath = @"C:\develop\workspace\TEMP\JournalData\test files\";

        protected override Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
#if DEBUG
            commandData.JournalData.Add(EditMetadataCommandData.KeyLibrary, DebugLibraryPath);
#endif
            if (JournalKeyExist(commandData, EditMetadataCommandData.KeyLibrary, out var library) == false)
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
            using (var transactionGroup = new TransactionGroup(Document, TransactionName))
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
