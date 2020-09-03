namespace RevitAction.Report.Message
{
    public class ErrorMessage : ReportMessage
    {
        public ErrorMessage() : base()
        {
            Kind = ReportKind.Error;
        }
    }
}
