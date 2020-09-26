using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Json;
using DataSource.Model.FileSystem;
using RevitAction.Revit;
using System;
using System.Diagnostics;
using System.IO;
using DS = DataSource.Model;

namespace RevitCommand.Families.Metadata
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class CreateMetadataCommand : ARevitActionCommand<CreateMetadataAction>
    {
        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var rootDirectory = PathFactory.Instance.CreateRoot(Action.Library.Value);
            var revitFile = PathFactory.Instance.Create<RevitFamilyFile>(Document.PathName);
            var dataSource = new MetadataJsonDataSource(revitFile);
            var metaFamily = new DS.Family.Family();
            if (dataSource.Exist)
            {
                metaFamily = dataSource.Read();
            }

            metaFamily.LibraryPath = Action.Library.Value;

            using (var group = new TransactionGroup(Document))
            {
                group.Start("Create Metadata");
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
