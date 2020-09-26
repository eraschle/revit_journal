using DataSource.Model.FileSystem;

namespace RevitJournal.Report
{
    public class TaskReportFile : JsonFile
    {
        public TaskReportFile()
        {
            AddSuffixes("Result");
        }
    }
}
