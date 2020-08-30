namespace RevitAction.Reports.Messages
{
    public enum ReportKind
    {
        Journal, Success, Error, Info
    }

    public interface IReportMessage
    {
        ReportKind Kind { get; }

        string Message { get; set; }
    }
}
