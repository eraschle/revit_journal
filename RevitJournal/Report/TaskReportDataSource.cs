using Newtonsoft.Json;
using System.IO;

namespace RevitJournal.Report
{
    public class TaskReportDataSource
    {
        public void Write(TaskReport result)
        {
            if (result is null) { return; }

            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var content = JsonConvert.SerializeObject(result, Formatting.Indented, setting);
            var resultFile = result.SourceFile.ChangeExtension<TaskReportFile>();
            File.WriteAllText(resultFile.FullPath, content);
        }
    }
}
