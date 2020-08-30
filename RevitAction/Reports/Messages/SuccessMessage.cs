namespace RevitAction.Reports.Messages
{
    public class SuccessMessage : AReportMessage, IReportMessage
    {
        public SuccessMessage() : base(ReportKind.Success) { }
        public SuccessMessage(string message) : base(ReportKind.Success, message) { }
    }
}
