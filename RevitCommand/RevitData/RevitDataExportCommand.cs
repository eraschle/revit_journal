using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.ProductData;
using Autodesk.Revit.Attributes;
using DataSource.Json;
using RevitAction.Revit;

namespace RevitCommand.RevitData
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class RevitDataExportCommand : ARevitActionCommand<RevitDataExportAction>
    {
        private readonly RevitEnumCreator Creator = new RevitEnumCreator();
        private const string ProductDataDirectory = @"C:\workspace\TEMP\RevitJournal\ProductData";

        protected override Result ExecuteRevitCommand(ref string message, ref string errorMessage)
        {
            if (int.TryParse(Application.VersionNumber, out var version) == false)
            {
                message = $"Could not parse version number: {Application.VersionNumber}";
                return Result.Failed;
            }
#if DEBUG
            Action.ExportDirectory.Value = ProductDataDirectory;
#endif
            var jsonFile = ProductDataJsonDataSource.CreateJsonFile(Action.ExportDirectory.Value, version);
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
