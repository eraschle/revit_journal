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
            var resultFile = GetResultFile(result);
            File.WriteAllText(resultFile.FullPath, content);
        }

        private TaskReportFile GetResultFile(TaskReport result)
        {
            return result.SourceFile
                         .ChangeExtension<TaskReportFile>(TaskReportFile.TaskResultExtension)
                         .ChangeFileName<TaskReportFile>(result.SourceFile.Name + "_Result");
        }
    }
}
