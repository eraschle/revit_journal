using RevitAction.Report;
using RevitAction.Report.Message;
using RevitAction.Report.Network;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace RevitJournal.Report.Network
{
    public class ReportClient<TResult> where TResult : class, IReportReceiver
    {
        private readonly ReceivePacket _receive;
        private readonly SendPacket _send;

        public Func<string, TResult> FindReport { get; set; }

        public TResult Reporter { get; private set; }

        public ReportClient(Socket socket)
        {
            _send = new SendPacket(socket);
            _receive = new ReceivePacket(socket)
            {
                ReportAction = ReportAction
            };
        }

        public void ReportAction(ReportMessage report)
        {
            if (report is null) { return; }

            switch (report.Kind)
            {
                case ReportKind.Status:
                    if (ActionManager.IsOpenAction(report.ActionId))
                    {
                        Reporter = FindReport.Invoke(report.Message);
                    }
                    if (Reporter is object)
                    {
                        var nextActionId = Reporter.GetNextAction(report.ActionId);
                        _send.Send(nextActionId.ToString());
                    }
                    break;
                case ReportKind.Error:
                default:
                    break;
            }
            Reporter?.MakeReport(report);
        }

        public string ClientId
        {
            get { return Reporter?.TaskId; }
        }

        public void StartReceiving()
        {
            _receive.StartReceiving();
        }

        public void StopReceiving()
        {
            _receive.StopReceiving();
        }
    }
}