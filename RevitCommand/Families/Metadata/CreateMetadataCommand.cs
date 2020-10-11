using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.DataSource.Json;
using DataSource.Models.FileSystem;
using RevitAction.Revit;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DS = DataSource.Models;

namespace RevitCommand.Families.Metadata
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class CreateMetadataCommand : ARevitActionCommand<CreateMetadataAction>
    {
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            PathFactory.Instance.CreateRoot(Action.Library.Value);
            var revitFile = PathFactory.Instance.Create<RevitFamilyFile>(Document.PathName);
            var jsonFile = revitFile.ChangeExtension<JsonFile>();
            var metaFamily = new DataSource.Model.Metadata.Family();
            var dataSource = new FamilyJsonDataSource();
            dataSource.SetFile(revitFile);
            if (jsonFile.Exists())
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
