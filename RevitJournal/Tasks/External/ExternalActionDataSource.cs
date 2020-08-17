using Newtonsoft.Json;
using System;
using System.IO;

namespace RevitJournal.Tasks.External
{
    public class ExternalActionDataSource
    {
        public ExternalAction Read(ExternalActionFile actionFile)
        {
            if (actionFile is null || actionFile.Exist == false)
            {
                throw new ArgumentException($"File does not exists {actionFile}");
            }
            var content = File.ReadAllText(actionFile.FullPath);
            return JsonConvert.DeserializeObject<ExternalAction>(content, GetSettings());
        }

        private JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public void Write(ExternalAction actions, ExternalActionFile actionFile)
        {
            if (actions is null) { throw new ArgumentNullException(nameof(actions)); }
            if (actionFile is null) { throw new ArgumentNullException(nameof(actionFile)); }

            var jsonData = JsonConvert.SerializeObject(actions, GetSettings());
            File.WriteAllText(actionFile.FullPath, jsonData);
        }
    }
}
