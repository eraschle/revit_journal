namespace RevitAction.Report.Message
{
    public class SuccessMessage : ReportMessage
    {
        public SuccessMessage() : base()
        {
            Kind = ReportKind.Success;
        }
    }
}
