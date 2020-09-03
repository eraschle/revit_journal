using RevitAction.Report.Message;
using System.Net;

namespace RevitAction.Report
{
    public interface IReportPublisher
    {
        void SendReport(ReportMessage report);

        void Connect(IPAddress address, short port);

        void Disconnect();
    }
}