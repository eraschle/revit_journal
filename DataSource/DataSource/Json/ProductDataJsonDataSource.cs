using DataSource.DataSource.Json;
using DataSource.Helper;
using DataSource.Metadata;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using DataSource.Model.ProductData;
using System.IO;

namespace DataSource.Json
{
    public static class ProductDataJsonDataSource
    {
        public const string JsonFileName = "RevitProductData";

        public static JsonDataSource<RevitProductData> CreateDataSource(JsonFile jsonFile)
        {
            return new JsonDataSource<RevitProductData>(jsonFile);
        }        

        public static JsonFile CreateJsonFile(string directory, int version)
        {
            var fileName = string.Concat(JsonFileName, Constant.Underline, version, Constant.Point, JsonFile.FileExtension);
            var filePath = Path.Combine(directory, fileName);
            return AFile.Create<JsonFile>(filePath);
        }
    }
}