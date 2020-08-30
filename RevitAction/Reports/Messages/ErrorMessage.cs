namespace RevitAction.Reports.Messages
{
    public class ErrorMessage : AReportMessage, IReportMessage
    {
        public ErrorMessage() : base(ReportKind.Error) { }
        public ErrorMessage(string message) : base(ReportKind.Error, message) { }
    }
}
