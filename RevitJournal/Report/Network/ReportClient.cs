using RevitAction.Report;
using RevitAction.Report.Message;
using RevitAction.Report.Network;
using RevitJournal.Tasks;
using System;
using System.Net.Sockets;

namespace RevitJournal.Report.Network
{
    public class ReportClient
    {
        private readonly ReceivePacket _receivePacket;
        private readonly SendPacket _sendPacket;

        public TaskManager TaskManager { get; set; }

        public IReportReceiver Reporter { get; private set; }

        public ActionManager ActionManager { get; private set; }

        public ReportClient(Socket socket)
        {
            _sendPacket = new SendPacket(socket);
            _receivePacket = new ReceivePacket(socket)
            {
                ReportAction = ReportAction
            };
        }

        public void ReportAction(ReportMessage report)
        {
            if (report is null) { return; }

            switch (report.Kind)
            {
                case ReportKind.DefaultAction:
                    if (ActionManager.IsOpenAction(report.ActionId))
                    {
                        Reporter = TaskManager.ByTaskId(report.Message);
                    }
                    break;
                case ReportKind.CustomAction:
                case ReportKind.Warning:
                case ReportKind.Error:
                default:
                    break;
            }
            SendResponseMessage(report);
            Reporter?.MakeReport(report);
        }

        private void SendResponseMessage(ReportMessage report)
        {
            if (report is null || Reporter is null) { return; }

            var responseMessage = string.Empty;
            if (ActionManager.IsInitialAction(report.ActionId))
            {
                ActionManager = TaskManager.GetActionManager();
                responseMessage = MessageUtils.Write(ActionManager);
            }
            else if (ActionManager is object)
            {
                var nextActionId = ActionManager.GetNextAction(report.ActionId);
                switch (report.Kind)
                {
                    case ReportKind.DefaultAction:
                    case ReportKind.CustomAction:
                    case ReportKind.Warning:
                    case ReportKind.Error:
                    default:
                        if (ActionManager.IsCustomFinish(report.Message) == false)
                        {
                            nextActionId = report.ActionId;
                        }
                        break;
                }
                responseMessage = nextActionId.ToString();
            }
            if (string.IsNullOrEmpty(responseMessage)) { return; }

            _sendPacket.Send(responseMessage);
        }

        public string ClientId
        {
            get { return Reporter?.TaskId; }
        }

        public void StartReceiving()
        {
            _receivePacket.StartReceiving();
        }

        public void StopReceiving()
        {
            _receivePacket.StopReceiving();
        }
    }
}