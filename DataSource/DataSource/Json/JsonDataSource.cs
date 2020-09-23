using DataSource.Helper;
using DataSource.Model.FileSystem;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DataSource.DataSource.Json
{
    public class JsonDataSource<TModel> where TModel : class
    {
        public JsonFile JsonFile { get; internal set; }

        public JsonDataSource(RevitFamilyFile revitFile)
        {
            if(revitFile is null) { throw new ArgumentNullException(nameof(revitFile)); }

            JsonFile = revitFile.ChangeExtension<JsonFile>();
        }

        public JsonDataSource(JsonFile jsonFile)
        {
            JsonFile = jsonFile;
        }

        public void AddFileNameSuffix(params string[] suffixes)
        {
            if (suffixes is null || suffixes.Length == 0) { return; }

            JsonFile.NameSuffixes.AddRange(suffixes);
        }

        public TModel Read(JsonFile source = null)
        {
            if (source is null)
            {
                source = JsonFile;
            }
            if(source.Exists() == false)
            {
                return null;
            }
            var content = File.ReadAllText(source.FullPath);
            var report = JsonConvert.DeserializeObject<TModel>(content, GetSettings());
            return report;
        }

        private JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public void Write(TModel model)
        {
            Write(model, JsonFile);
        }

        public void Write(TModel model, JsonFile destination)
        {
            if (model is null) { return; }

            var jsonData = JsonConvert.SerializeObject(model, GetSettings());
            File.WriteAllText(destination.FullPath, jsonData);
        }
    }
}
