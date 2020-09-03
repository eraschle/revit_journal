namespace RevitAction.Reports.Report
{
    public enum ReportKind
    {
        Open, Save, SaveAs, Close, Journal, Success, Error, Info
    }

    public interface IReportMessage
    {
        ReportKind Kind { get; }

        string Report { get; set; }
    }
}
