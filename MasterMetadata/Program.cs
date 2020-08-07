using DataSource.DataSource.Json;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;

namespace MasterMetadata
{
    class Program
    {
        public const string RootDir = @"C:\workspace\TEMP\Metadata";
        public const string OutputFile = @"C:\workspace\TEMP\master_not_indent.json";
        public const string Extension = "*.json";

        private static JsonDataSource<Family> FamilyDao;

        static void Main(string[] args)
        {
            var files = Directory.GetFiles(RootDir, Extension, SearchOption.AllDirectories);
            var families = new List<Family>();
            try
            {
            foreach (var filePath in files)
            {
                var jsonFile = AFile.Create<JsonFile>(filePath);
                if(FamilyDao is null)
                {
                    FamilyDao = new JsonDataSource<Family>(jsonFile);
                }
                var family = FamilyDao.Read(jsonFile);
                Console.WriteLine("Read File: " + jsonFile.NameWithExtension);
                families.Add(family);
            }
            var masterJson = AFile.Create<JsonFile>(OutputFile);
            var masteDao = new JsonDataSource<List<Family>>(masterJson);
            masteDao.Write(families);
            Console.WriteLine("Created Master JSON");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.WriteLine("Press any Key to close");
            Console.ReadLine();
        }
    }
}
