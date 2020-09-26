using DataSource.DataSource.Json;
using Newtonsoft.Json;
using System.IO;

namespace RevitJournal.Report
{
    public class TaskReportDataSource : JsonDataSource<TaskReport>
    {
        public override TaskReport Read()
        {
            throw new System.NotImplementedException();
        }

        public override void Write(TaskReport result)
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
