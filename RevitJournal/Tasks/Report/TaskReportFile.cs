using DataSource.Model.FileSystem;

namespace RevitJournal.Tasks.Report
{
    public class TaskReportFile : AFile
    {
        public const string TaskResultExtension = "json";

        protected override string GetExtension()
        {
            return TaskResultExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(TaskReportFile);
        }
    }
}
