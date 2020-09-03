namespace RevitAction.Report.Message
{
    public class ErrorMessage : ReportMessage
    {
        public ErrorMessage() : base(ReportKind.Error) { }
        public ErrorMessage(string message) : base(ReportKind.Error, message) { }
    }
}
