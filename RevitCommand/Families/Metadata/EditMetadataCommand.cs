using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Models.FileSystem;
using RevitAction.Revit;
using System;
using System.Diagnostics.CodeAnalysis;

namespace RevitCommand.Families.Metadata
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class EditMetadataCommand : ARevitActionCommand<EditMetadataAction>
    {
        private const string debugLibraryPath = @"C:\develop\workspace\TEMP\JournalData\test files\";

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
#if DEBUG
            Action.Library.Value = debugLibraryPath;
#endif
            var rootDirectory = PathFactory.Instance.CreateRoot(Action.Library.Value);
            var revitFile = PathFactory.Instance.Create<RevitFamilyFile>(Document.PathName);
            if(revitFile.HasEditMetadata == false)
            {
                message = "No Edited metadata file exists";
                return Result.Failed;
            }
            revitFile.SetExternalEditDataSource();
            revitFile.Update();
            var metaFamily = revitFile.Metadata;
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
