using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using RevitAction.Report;
using System;

namespace RevitAction.Revit
{
    public class TaskApp : IExternalApplication
    {
        public static ReportManager Reporter { get; private set; }

        private ControlledApplication application;

        public Result OnStartup(UIControlledApplication uiApplication)
        {
            if (uiApplication is null) { return Result.Failed; }

            application = uiApplication.ControlledApplication;

            Reporter = new ReportManager(application);
            Reporter.Connect();
            if (Reporter.InitialReport() == false)
            {
               Reporter.AddJournalComment<TaskApp>("No action manager received");
            }

            uiApplication.ApplicationClosing += Application_ApplicationClosing;
            uiApplication.DialogBoxShowing += Application_DialogBoxShowing;

            application.DocumentOpened += ControlledApplication_DocumentOpened;

            application.DocumentSaved += ControlledApplication_DocumentSaved;
            application.DocumentSavedAs += ControlledApplication_DocumentSavedAs;

            application.FailuresProcessing += ControlledApplication_FailuresProcessing;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication uiApplication)
        {

            uiApplication.ApplicationClosing -= Application_ApplicationClosing;
            uiApplication.DialogBoxShowing -= Application_DialogBoxShowing;

            var application = uiApplication.ControlledApplication;
            application.DocumentOpened -= ControlledApplication_DocumentOpened;

            application.DocumentSaved -= ControlledApplication_DocumentSaved;
            application.DocumentSavedAs -= ControlledApplication_DocumentSavedAs;

            application.FailuresProcessing -= ControlledApplication_FailuresProcessing;
            return Result.Succeeded;
        }



        #region Default Action Events

        #region Open Events

        private void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            if (Reporter.OpenReport(args.Document.PathName))
            {
                Reporter.JournalReport(args.Document.Application.RecordingJournalFilename);
            }
            else
            {
                Reporter.CurrentActionId = ActionManager.OpenActionId;
                var message = $"Server could not find task for {args.Document.PathName}";
                Reporter.AddJournalComment<TaskApp>(message);
                Reporter.ErrorReport(message);
            }
        }

        #endregion

        #region Save Events

        private void ControlledApplication_DocumentSaved(object sender, DocumentSavedEventArgs args)
        {
            Reporter.SaveReport(args.Document.PathName);
        }

        private void ControlledApplication_DocumentSavedAs(object sender, DocumentSavedAsEventArgs args)
        {
            Reporter.SaveReport(args.Document.PathName);
        }

        #endregion

        #region Close Events

        private void Application_ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
            var message = "Disconnected from server";
            try
            {
                Reporter.Disconnect();
            }
            catch (Exception exception)
            {
                message += $" Exception: {exception.Message}";
            }
            Reporter.AddJournalComment<TaskApp>(message);
        }

        #endregion

        #region Error Events

        private void Application_DialogBoxShowing(object sender, DialogBoxShowingEventArgs args)
        {
            if (Reporter.IsAllowdDialog(args.DialogId, out var handler))
            {
                Reporter.CurrentActionId = handler.ActionId;
                var result = args.OverrideResult(handler.ButtonId);
                if (result == false)
                {
                    Reporter.AddJournalComment<TaskApp>($"\"{args.DialogId}\" result code not accepted: Action: {handler.ActionId} >> {handler.ButtonId}");
                }
            }
            else
            {
                var result = args.OverrideResult(-1);
                Reporter.AddJournalComment<TaskApp>($"No handler for Dialog \"{args.DialogId}\" result code accepted?: {result}");
                Reporter.ErrorReport(args.DialogId);
            }
        }

        private void ControlledApplication_FailuresProcessing(object sender, FailuresProcessingEventArgs args)
        {
            var result = args.GetProcessingResult();
            if (result == FailureProcessingResult.WaitForUserInput)
            {
                Reporter.ErrorReport(result.ToString());
            }
        }

        #endregion

        #endregion

    }
}
