using RevitAction.Report.Message;
using System;
using System.Net;

namespace RevitAction.Report
{
    public interface IReportPublisher
    {
        void SendReport(ReportMessage report);

        Guid GetActionIdResponse();

        ActionManager GetActionManagerResponse();

        void Connect(IPAddress address, short port);

        void Disconnect();
    }
}