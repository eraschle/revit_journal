using Autodesk.Revit.ApplicationServices;
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

        public ReportManager(IPAddress address = null, short port = 8888)
        {
            _serverAddress = address ?? IPAddress.Loopback;
            _serverPort = port;
        }

        public ReportPublisher Publisher { get; private set; }

        public Guid CurrentActionId { get; set; }

        public ITaskActionCommand CostumAction { get; private set; }

        public void SetCostumAction(ITaskActionCommand taskAction)
        {
            CostumAction = taskAction ?? throw new ArgumentNullException(nameof(taskAction));
            CurrentActionId = taskAction.ActionId;
        }

        public ActionManager TaskActions { get; set; }

        internal bool InitialReport()
        {
            CurrentActionId = ActionManager.InitialActionId;
            var report = GetReport(ActionManager.InitialMessage, ReportKind.DefaultAction);
            Publisher.SendReport(report);
            TaskActions = Publisher.GetResponsedTaskActions();
            return TaskActions is object;
        }

        internal bool OpenReport(string message)
        {
            DefaultReport(ActionManager.OpenActionId, message);
            return CurrentActionId.Equals(ActionManager.JournalActionId);
        }

        internal void JournalReport(string message)
        {
            DefaultReport(ActionManager.JournalActionId, message);
        }

        internal void SaveReport(string message)
        {
            DefaultReport(ActionManager.SaveActionId, message);
        }

        private void DefaultReport(Guid actionId, string message)
        {
            CurrentActionId = actionId;
            Report(message, ReportKind.DefaultAction);
        }

        public void CustomStartReport()
        {
            CustomReport(ActionManager.CustomStartMessage);
        }

        public void CustomFinishReport()
        {
            CustomReport(ActionManager.CustomFinishMessage);
        }

        public void CustomReport(string message)
        {
            Report(message, ReportKind.CustomAction);
        }

        public void ErrorReport(string message, Exception exception = null)
        {
            Report(message, ReportKind.Error, exception);
        }

        public void WarningReport(string message)
        {
            Report(message, ReportKind.Warning);
        }

        private void Report(string message, ReportKind reportKind, Exception exception = null)
        {
            var report = GetReport(message, reportKind, exception);
            Publisher.SendReport(report);
            CurrentActionId = Publisher.GetResponsedActionId();
        }

        private ReportMessage GetReport(string message, ReportKind reportKind, Exception exception = null)
        {
            return new ReportMessage
            {
                ActionId = GetActionId(),
                ActionName = GetActionName(),
                Kind = reportKind,
                Message = message,
                Exception = exception
            };
        }

        private Guid GetActionId()
        {
            return TaskActions.IsCostumAction(CurrentActionId)
                ? CostumAction.ActionId
                : CurrentActionId;
        }

        private string GetActionName()
        {
            return TaskActions.GetActionName(CurrentActionId);
        }

        public bool IsAllowdDialog(string dialogId, out DialogHandler dialogHandler)
        {
            return TaskActions.IsAllowedDialogs(dialogId, out dialogHandler);
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
