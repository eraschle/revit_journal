using Newtonsoft.Json;
using System;

namespace RevitAction.Report.Message
{
    public class MessageUtils
    {
        public static ReportMessage ReadReport(string content)
        {
            return JsonConvert.DeserializeObject<ReportMessage>(content, GetSettings());
        }

        public static ActionManager ReadTasks(string content)
        {
            return JsonConvert.DeserializeObject<ActionManager>(content, GetSettings());
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

        public static string Write(ActionManager actionManager)
        {
            if (actionManager is null) { throw new ArgumentNullException(nameof(actionManager)); }

            return JsonConvert.SerializeObject(actionManager, GetSettings());
        }
    }
}
