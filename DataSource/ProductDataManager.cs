using DataSource.DataSource.Json;
using DataSource.Json;
using DataSource.Model.Catalog;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using DataSource.Model.ProductData;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataSource
{
    public class ProductDataManager
    {
        private static ProductDataManager Instance;

        public static void Create(string directory)
        {
            if (Instance != null) { return; }

            if (string.IsNullOrWhiteSpace(directory) || Directory.Exists(directory) == false)
            {
                throw new ArgumentNullException($"{nameof(directory)} is null or does not exists");
            }
            var root = PathFactory.Instance.CreateRoot(directory);
            Instance = new ProductDataManager { ProductDataDirectory = root };
        }

        public static ProductDataManager Get()
        {
#if DEBUG
            var directory = @"C:\develop\workspace\revit_journal_test_data\productData";
            Create(directory);
#endif
            if (Instance is null)
            {
                throw new ArgumentNullException("Product Manager has not been creaeted");
            }
            return Instance;
        }

        private readonly Dictionary<int, RevitProductData> RevitProductDatas = new Dictionary<int, RevitProductData>();

        private JsonDataSource<RevitProductData> JsonDataSource;

        private DirectoryNode ProductDataDirectory { get; set; }

        private IList<RevitParameterGroup> _ParameterGroups = new List<RevitParameterGroup>();
        public IList<RevitParameterGroup> ParameterGroups()
        {
            var oldest = ProductManager.OldestVersion();
            if (HasByVersion(oldest, out var productdata))
            {
                return productdata.ParameterGroups;
            }
            return new List<RevitParameterGroup>();
        }

        public void AddData(RevitApp revitApp)
        {
            if (revitApp is null) { return; }

            var version = revitApp.Version;
            if (RevitProductDatas.ContainsKey(version) == false)
            {
                var jsonFile = ProductDataJsonDataSource.CreateJsonFile(ProductDataDirectory, revitApp.Version);
                if (JsonDataSource is null)
                {
                    JsonDataSource = new JsonDataSource<RevitProductData>(jsonFile);
                }
                JsonDataSource.JsonFile = jsonFile;
                var productData = JsonDataSource.Read();
                RevitProductDatas.Add(version, productData);
            }
        }

        public bool HasByVersion(RevitApp revitApp, out RevitProductData productData)
        {
            productData = ByVersion(revitApp);
            return productData != null;
        }

        public RevitProductData ByVersion(RevitApp revitApp)
        {
            RevitProductData productData = null;
            if (RevitProductDatas.ContainsKey(revitApp.Version) == false)
            {
                AddData(revitApp);
            }

            if (RevitProductDatas.ContainsKey(revitApp.Version))
            {
                productData = RevitProductDatas[revitApp.Version];
                if (productData != null)
                {
                    productData.SetDefaultData(revitApp);
                }
            }
            return productData;
        }
    }
}
