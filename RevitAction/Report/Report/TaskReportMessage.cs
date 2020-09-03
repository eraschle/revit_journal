namespace RevitAction.Reports.Report
{
    public class TaskReportMessage : IReportMessage
    {
        public ReportKind Kind { get; private set; }

        public string Report { get; set; }

        public TaskReportMessage(ReportKind kind)
        {
            Kind = kind;
        }
        public TaskReportMessage(ReportKind kind, string report) : this(kind)
        {
            Report = report;
        }
    }
}
