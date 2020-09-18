using Autodesk.Revit.ApplicationServices;
using RevitAction.Action;
using RevitAction.Report.Message;
using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace RevitAction.Report
{
    public class ReportManager
    {
        private const string JournalPrefix = "Task Actions";

        private readonly IPAddress _serverAddress;
        private readonly short _serverPort;

        private readonly ControlledApplication _application;

        public ReportManager(ControlledApplication application, IPAddress address = null, short port = 8888)
        {
            _application = application;
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
            var report = GetReport(ActionManager.InitialMessage, ReportKind.Message);
            Publisher.SendReport(report);
            TaskActions = Publisher.GetActionManagerResponse();
            return TaskActions is object;
        }

        public void AddJournalComment<TSource>(string message, Exception exception = null,
                                               [CallerMemberName] string memberName = "",
                                               [CallerLineNumber] int lineNumber = 0) where TSource : class
        {
            var comment = $"{JournalPrefix}: [{lineNumber}] {typeof(TSource).Name}.{memberName} >> {message}";
            if (exception is object)
            {
                comment += $" Exception: {exception.Message}";
            }
            _application.WriteJournalComment(comment, true);
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
            Report(message, ReportKind.Message);
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
            Report(message, ReportKind.Message);
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
            CurrentActionId = Publisher.GetActionIdResponse();
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
            return TaskActions is null || TaskActions.HasDialogAction(CurrentActionId, out var action) == false
                ? CurrentActionId
                : action.ActionId;
        }

        private string GetActionName()
        {
            return TaskActions is null 
                ? ActionManager.InitialActionName 
                : TaskActions.GetActionName(CurrentActionId);
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
