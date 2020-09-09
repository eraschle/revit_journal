using RevitAction.Report;
using RevitAction.Report.Message;
using RevitAction.Report.Network;
using System;
using System.Net.Sockets;

namespace RevitJournal.Report
{
    public class ReportClient<TResult> where TResult : class, IReportReceiver
    {
        private static readonly ActionManager actionManager = new ActionManager();

        private readonly ReceivePacket _receive;
        private readonly SendPacket sendPacket;

        public Func<string, TResult> FindReport { get; set; }

        public IProgress<TResult> Progress { get; set; }

        public TResult Reporter { get; private set; }

        public ReportClient(Socket socket)
        {
            sendPacket = new SendPacket(socket);
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
                    if (actionManager.IsOpenAction(report.ActionId))
                    {
                        Reporter = FindReport.Invoke(report.Message);
                    }
                    if (Reporter is object)
                    {
                        var nextActionId = Reporter.GetNextAction(report.ActionId);
                        sendPacket.Send(nextActionId.ToString());
                    }
                    break;
                case ReportKind.Error:
                default:
                    break;
            }
            if (Reporter is null) { return; }

            Reporter.MakeReport(report);
            Progress.Report(Reporter);
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