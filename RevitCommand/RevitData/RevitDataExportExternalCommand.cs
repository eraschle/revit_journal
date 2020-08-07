using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Cat = DataSource.Model.Catalog;
using DataSource.Model.ProductData;
using System;
using Autodesk.Revit.Attributes;
using DataSource.Json;
using DataSource.Model.FileSystem;

namespace RevitCommand.RevitData
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class RevitDataExportExternalCommand : ARevitExternalCommand
    {
        private readonly RevitEnumCreator Creator = new RevitEnumCreator();
        private const string ProductDataDirectory = @"C:\workspace\TEMP\RevitJournal\ProductData";

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if(int.TryParse(Application.VersionNumber, out var version) == false)
            {
                message = $"Could not parse version number: {Application.VersionNumber}";
                return Result.Failed;
            }

            var journalKey = RevitDataExportCommandData.ProductDataDir;
#if DEBUG
            commandData.JournalData.Add(journalKey, ProductDataDirectory);
#endif
            if (JournalKeyExist(commandData, journalKey, out var productDirectory) == false)
            {
                message = $"No Journal Key for Directory of ProductData found: {journalKey}";
                return Result.Failed;
            }

            var jsonFile = ProductDataJsonDataSource.CreateJsonFile(productDirectory, version);
            var datasource = ProductDataJsonDataSource.CreateDataSource(jsonFile);
            var productData = new RevitProductData
            {
                Name = Application.VersionName,
                Version = version
            };

            productData = Creator.GetBuiltInParameters(productData);
            productData = Creator.GetBuiltInParameterGroup(productData);
            productData = Creator.GetParameterType(productData);
            productData = Creator.GetDisplayUnitTypes(productData);
            productData = Creator.GetUnitSymbolTypes(productData);
            productData = Creator.GetUnitTypes(productData);
            productData = Creator.GetFamilyPlacementTypes(productData);
            productData = GetCategories(productData);

            datasource.Write(productData);

            return Result.Succeeded;
        }


        private RevitProductData GetCategories(RevitProductData productData)
        {
            foreach (Category category in Document.Settings.Categories)
            {
                if (category.CategoryType != CategoryType.Model
                    && category.CategoryType != CategoryType.Annotation)
                {
                    continue;
                }

                var model = Creator.CreateCategory(category);
                productData.Categories.Add(model);
            }
            return productData;
        }


    }
}
