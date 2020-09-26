using DataSource.DataSource.Json;
using DataSource.Helper;
using DataSource.Model.FileSystem;
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

        public static JsonFile CreateJsonFile(DirectoryNode directory, int version)
        {
            var fileName = string.Concat(JsonFileName, Constant.Underline, version, Constant.Point, JsonFile.JsonExtension);
            var filePath = Path.Combine(directory.FullPath, fileName);
            return PathFactory.Instance.Create<JsonFile>(filePath);
        }
    }
}