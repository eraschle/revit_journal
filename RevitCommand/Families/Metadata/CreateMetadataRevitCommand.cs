using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Helper;
using DataSource.Json;
using DataSource.Model.FileSystem;
using RevitJournal.Revit.Commands.Parameter;
using System;
using System.Diagnostics;
using DS = DataSource.Model;

namespace RevitCommand.Families.Metadata
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class CreateMetadataRevitCommand : AFamilyRevitCommand
    {
        private readonly CreateMetadataAction action = new CreateMetadataAction();

        protected override Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var revitFile = AFile.Create<RevitFamilyFile>(Document.PathName);
            var dataSource = new MetadataJsonDataSource(revitFile);
            var metaFamily = new DS.Family.Family();
            if (dataSource.Exist)
            {
                metaFamily = dataSource.Read();
            }

            if (JournalKeyExist(commandData, action.Library.JournalKey, out var libraryPath))
            {
                metaFamily.LibraryPath = ModelHelper.GetLibraryPath(revitFile, libraryPath);
            }

            using (var group = new TransactionGroup(Document, "Create Metadata"))
            {
                group.Start();
                var metaManager = new RevitMetadataManager(Document);
                try
                {
                    metaManager.MergeFamily(metaFamily);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    group.RollBack();
                }
            }
            dataSource.Write(metaFamily);
            return Result.Succeeded;
        }
    }
}
