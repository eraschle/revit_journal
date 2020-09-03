namespace RevitAction.Report.Message
{
    public class InfoMessage : AReportMessage, IReportMessage
    {
        public InfoMessage() : base(ReportKind.Info) { }
        public InfoMessage(string message) : base(ReportKind.Info, message) { }
    }
}
