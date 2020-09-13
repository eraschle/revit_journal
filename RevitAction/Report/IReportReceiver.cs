using RevitAction.Report.Message;
using System;

namespace RevitAction.Report
{
    public interface IReportReceiver
    {
        string TaskId { get; }

        void MakeReport(ReportMessage report);

        Action<string> DisconnectAction { get; set; }
    }
}