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

        public ActionManager ActionManager { get; internal set; }

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

            if (ActionManager.IsOpenAction(report.ActionId))
            {
                Reporter = TaskManager.ByTaskId(report.Message);
            }
            SendResponseMessage(report);
            Reporter?.MakeReport(report);
        }

        private void SendResponseMessage(ReportMessage report)
        {
            if (report is null) { return; }

            string responseMessage;
            if (ActionManager.IsInitialAction(report.ActionId))
            {
                responseMessage = MessageUtils.Write(ActionManager);
            }
            else
            {
                var nextActionId = ActionManager.GetNextAction(report.ActionId);
                switch (report.Kind)
                {
                    case ReportKind.Message:
                    case ReportKind.Warning:
                        if (ActionManager.IsCostumnAction(report.ActionId) 
                            && ActionManager.IsCustomFinish(report.Message) == false)
                        {
                            nextActionId = report.ActionId;
                        }
                        break;
                    case ReportKind.Error:
                    default:
                        break;
                }
                responseMessage = nextActionId.ToString();
            }
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