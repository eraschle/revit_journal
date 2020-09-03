using RevitAction.Report.Message;
using System;
using System.Net;

namespace RevitAction.Report
{
    public class ReportManager
    {
        private readonly IPAddress _serverAddress;
        private readonly short _serverPort;

        public static Guid OpenActionId { get; } = new Guid("b76c8f93-b406-42ab-93a5-8a155ba04641");

        public static Guid SaveActionId { get; } = new Guid("5dd16def-6869-4747-a660-113f5a273922");

        public static Guid SaveAsActionId { get; } = new Guid("1848a457-4743-4f35-ae4d-9a85264237b2");

        public static Guid CloseActionId { get; } = new Guid("63d85178-d717-4b4b-8333-25e9aef02284");

        public ReportManager(IPAddress address = null, short port = 8888)
        {
            _serverAddress = address ?? IPAddress.Loopback;
            _serverPort = port;
        }

        public ReportPublisher Publisher { get; private set; }

        public string FilePath { get; set; }

        public string Journal { get; set; }

        public Guid ActionId { get; set; }

        public void StartAction()
        {
            ActionId = OpenActionId;
            var report = new ReportMessage { Kind = ReportKind.Start };
            Send(report);
        }

        public void OpenAction(string message)
        {
            ActionId = OpenActionId;
            var report = new ReportMessage { Message = message };
            Send(report);
        }

        public void SaveAction(string message)
        {
            ActionId = SaveActionId;
            var report = new ReportMessage { Message = message };
            Send(report);
        }

        public void SaveAsAction(string message)
        {
            ActionId = SaveAsActionId;
            var report = new SuccessMessage { Message = message };
            Send(report);
        }

        public void CloseAction(string message)
        {
            ActionId = CloseActionId;
            var report = new SuccessMessage { Message = message };
            Send(report);
        }

        public void Success(string message)
        {
            var report = new SuccessMessage { Message = message };
            Send(report);
        }

        public void Info(string message)
        {
            var report = new ReportMessage { Kind = ReportKind.Success, Message = message };
            Send(report);
        }

        public void Error(string message, Exception exception = null)
        {
            if (exception != null)
            {
                message = string.Join(Environment.NewLine, message, exception.Message, exception.StackTrace);
            }
            var report = new ErrorMessage(message);
            Send(report);
        }

        private void Send(ReportMessage report)
        {
            if (report is null) { throw new ArgumentNullException(nameof(report)); }

            if (ActionId == OpenActionId)
            {
                FilePath = report.Message;
            }
            report.FilePath = FilePath;
            report.ActionId = ActionId;
            report.Journal = Journal;

            Console.WriteLine($"Send data: {report}");
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
