using DataSource.Helper;
using DataSource.Model.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSource.DataSource.Json
{
    public class JsonDataSource<TModel> where TModel : class
    {
        public JsonFile JsonFile { get; internal set; }

        public JsonDataSource(RevitFamilyFile revitFile)
        {
            JsonFile = revitFile.ChangeExtension<JsonFile>(JsonFile.FileExtension);
        }

        public JsonDataSource(JsonFile jsonFile)
        {
            JsonFile = jsonFile;
        }

        public void AddFileNameSuffix(params string[] suffixes)
        {
            if (suffixes is null || suffixes.Length == 0) { return; }

            var suffix = string.Join(Constant.Underline, suffixes);
            var newFileName = string.Concat(JsonFile.Name, Constant.Underline, suffix);
            JsonFile = JsonFile.ChangeFileName<JsonFile>(newFileName);
        }

        public TModel Read(JsonFile source = null)
        {
            if (source is null)
            {
                source = JsonFile;
            }
            if(source.Exist == false)
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

            string jsonData = JsonConvert.SerializeObject(model, GetSettings());
            File.WriteAllText(destination.FullPath, jsonData);
        }
    }
}
