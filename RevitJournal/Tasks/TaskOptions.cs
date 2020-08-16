using RevitJournal.Tasks.Options;

namespace RevitJournal.Tasks
{
    public class TaskOptions
    {
        public CommonOptions Common { get; set; } = new CommonOptions();

        public ReportOptions Report { get; set; } = new ReportOptions();

        public BackupOptions Backup { get; set; } = new BackupOptions();

        public ParallelOptions Parallel { get; set; } = new ParallelOptions();

        public RevitArguments Arguments { get; set; } = new RevitArguments();

    }
}
