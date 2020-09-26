using DataSource.Model.FileSystem;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DataSource.DataSource.Json
{
    public class JsonDataSource<TModel> : AFileDataSource<TModel, JsonFile> where TModel : class
    {
        public override TModel Read()
        {
            if (FileNode.Exists() == false) { throw new ArgumentException($"File does not exists {FileNode.FullPath}"); }

            var content = File.ReadAllText(FileNode.FullPath);
            return JsonConvert.DeserializeObject<TModel>(content, GetSettings());
        }

        protected virtual JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public override void Write(TModel model)
        {
            if (model is null) { throw new ArgumentNullException(nameof(model)); }

            var jsonData = JsonConvert.SerializeObject(model, GetSettings());
            File.WriteAllText(FileNode.FullPath, jsonData);
        }
    }
}
