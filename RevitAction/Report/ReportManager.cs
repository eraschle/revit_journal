using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using RevitAction.Action;
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

        public Guid ActionId { get; set; }

        public void StartAction()
        {
            var report = new ReportMessage { Kind = ReportKind.Unknown };
            Send(report);
        }

        public bool OpenReport(Document document)
        {
            var report = new ReportMessage
            {
                Kind = ReportKind.Open,
                Message = document.PathName
            };
            Send(report);
            return Publisher.HasResponsed(report);
        }

        public void JournalReport(Document document)
        {
            var report = new ReportMessage
            {
                Kind = ReportKind.Journal,
                Message = document.Application.RecordingJournalFilename
            };
            Send(report);
        }

        public void SaveReport(Document document)
        {
            var report = new ReportMessage
            {
                Kind = ReportKind.Save,
                Message = document.PathName
            };
            Send(report);
        }

        public void SaveAsReport(Document document)
        {
            var report = new ReportMessage
            {
                Kind = ReportKind.SaveAs,
                Message = document.PathName
            };
            Send(report);
        }

        public bool CloseReport()
        {
            var report = new ReportMessage
            {
                Kind = ReportKind.Close,
                Message = "Closed"
            };
            Send(report);
            return Publisher.HasResponsed(report);
        }

        public void SuccessReport(string message)
        {
            var report = new SuccessMessage { Message = message };
            Send(report);
        }

        public void ActionStatusReport(ActionStatus status)
        {
            var report = new ReportMessage
            {
                Kind = ReportKind.Status,
                Message = status.ToString()
            };
            Send(report);
        }

        public void Error(string message, Exception exception = null)
        {
            if (exception != null)
            {
                message = string.Join(Environment.NewLine, message, exception.Message, exception.StackTrace);
            }
            var report = new ErrorMessage
            {
                Kind = ReportKind.Error,
                Message = message
            };
            Send(report);
        }

        private void Send(ReportMessage report)
        {
            if (report is null) { throw new ArgumentNullException(nameof(report)); }

            report.ActionId = ActionId;
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
