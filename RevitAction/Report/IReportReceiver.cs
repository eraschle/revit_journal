using RevitAction.Report.Message;

namespace RevitAction.Report
{
    public interface IReportReceiver
    {
        void SetStatus(int status);

        void MakeReport(ReportMessage report);

        string TaskId { get; }
    }
}