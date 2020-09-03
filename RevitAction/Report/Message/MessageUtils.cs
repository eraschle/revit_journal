using Newtonsoft.Json;
using System;

namespace RevitAction.Report.Message
{
    public class MessageUtils
    {
        public static ReportMessage Read(string content)
        {
            return JsonConvert.DeserializeObject<ReportMessage>(content, GetSettings());
        }

        private static JsonSerializerSettings GetSettings()
        {
            return new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static string Write(ReportMessage actions)
        {
            if (actions is null) { throw new ArgumentNullException(nameof(actions)); }

            return JsonConvert.SerializeObject(actions, GetSettings());
        }
    }
}
