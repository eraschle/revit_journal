using System.Net;

namespace RevitAction.Reports
{
    public interface IReportReceiver
    {
        void MakeReport(ReportData report);

        string GetId();
    }
}