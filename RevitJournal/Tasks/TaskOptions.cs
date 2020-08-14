using RevitJournal.Tasks.Options;

namespace RevitJournal.Tasks
{
    public class TaskOptions
    {
        public CommonOptions Common { get; set; }

        public ReportOptions Report { get; set; }

        public BackupOptions Backup { get; set; }

        public ParallelOptions Parallel { get; set; }

        public RevitArguments Arguments { get; set; }

    }
}
