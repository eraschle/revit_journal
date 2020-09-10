using RevitAction.Action;
using RevitAction.Report.Message;
using System;
using System.Net;

namespace RevitAction.Report
{
    public class ReportManager
    {
        private readonly static ActionManager actionManager = new ActionManager();

        private readonly IPAddress _serverAddress;
        private readonly short _serverPort;

        public ReportManager(IPAddress address = null, short port = 8888)
        {
            _serverAddress = address ?? IPAddress.Loopback;
            _serverPort = port;
        }

        public ReportPublisher Publisher { get; private set; }

        public Guid ActionId { get; set; } = actionManager.InitialActionId;

        internal void InitialReport()
        {
            var report = new ReportMessage
            {
                ActionId = ActionId,
                Kind = ReportKind.Status,
                Message = "Initial"
            };
            Publisher.SendReport(report);
        }

        internal bool OpenReport(string message)
        {
            ActionId = actionManager.OpenActionId;
            StatusReport(message);
            return ActionId.Equals(actionManager.JournalActionId);
        }

        internal void JournalReport(string message)
        {
            StatusReport(message);
        }

        internal void SaveReport(string message)
        {
            StatusReport(message);
        }

        public void StatusReport(string message)
        {
            var report = new ReportMessage
            {
                ActionId = ActionId,
                Kind = ReportKind.Status,
                Message = message
            };
            Publisher.SendReport(report);
            ActionId = Publisher.GetResponsed();
        }

        public void Error(string message, Exception exception = null)
        {
            var report = new ReportMessage
            {
                ActionId = ActionId,
                Kind = ReportKind.Error,
                Message = message,
                Exception = exception
            };
            Publisher.SendReport(report);
        }
        public void Warning(string message)
        {
            var report = new ReportMessage
            {
                ActionId = ActionId,
                Kind = ReportKind.Warning,
                Message = message
            };
            Publisher.SendReport(report);
        }

        public void Connect()
        {
            if (Publisher != null) { return; }

            Publisher = new ReportPublisher();
            Publisher.Connect(_serverAddress, _serverPort);
        }

        public void Disconnect()
        {
            Publisher.Disconnect();
        }
    }
}
