namespace RevitAction.Report.Message
{
    public abstract class AReportMessage : IReportMessage
    {
        public ReportKind Kind { get; private set; }

        public string Message { get; set; }

        protected AReportMessage(ReportKind kind)
        {
            Kind = kind;
        }
        protected AReportMessage(ReportKind kind, string report) : this(kind)
        {
            Message = report;
        }
    }
}
