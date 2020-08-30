namespace RevitAction.Reports.Messages
{
    public class TasMessage : AReportMessage, IReportMessage
    {
        public TasMessage(ReportKind kind) : base(kind) { }

        public TasMessage(ReportKind kind, string message) : base(kind, message) { }
    }
}
