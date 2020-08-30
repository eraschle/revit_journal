using System.Net;

namespace RevitAction.Reports
{
    public interface IReportPublisher
    {
        void SendReport(ReportData report);

        void Connect(IPAddress address, short port);

        void Disconnect();
    }
}