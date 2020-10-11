using DataSource.DataSource.Json;
using DataSource.Models.Catalog;
using DataSource.Models.FileSystem;
using DataSource.Models.Product;
using DataSource.Models.ProductData;
using System;
using System.Collections.Generic;

namespace DataSource
{
    public class ProductDataManager
    {
        private static ProductDataManager Instance;

        public static void Create(string directory)
        {
            if (Instance is object) { return; }

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }
            var dataDirectory = PathFactory.Instance.CreateRoot(directory);
            if (dataDirectory.Exists() == false)
            {
                dataDirectory.Create();
            }
            Instance = new ProductDataManager { RootDataDirectory = dataDirectory };
        }

        public static ProductDataManager Get()
        {
#if DEBUG
            var directory = @"C:\develop\workspace\revit_journal_test_data\productData";
            Create(directory);
#endif
            if (Instance is null)
            {
                throw new ArgumentNullException(nameof(ProductDataManager));
            }
            return Instance;
        }

        private readonly Dictionary<int, RevitProductData> RevitProductDatas = new Dictionary<int, RevitProductData>();

        private DirectoryNode RootDataDirectory { get; set; }

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
            var fileName = RevitProductData.GetFileName(revitApp.Version);
            var jsonFile = PathFactory.Instance.Create<JsonFile>(fileName, RootDataDirectory);
            if (jsonFile.Exists() == false)
            {
                jsonFile.RemoveParent();
                return;
            }

            if (RevitProductDatas.ContainsKey(version) == false)
            {
                var dataSource = new JsonDataSource<RevitProductData>();
                dataSource.SetFile(jsonFile);
                var productData = dataSource.Read();
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
