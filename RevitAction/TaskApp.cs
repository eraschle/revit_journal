using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using RevitAction.Report;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RevitAction
{
    public class TaskApp : IExternalApplication
    {
        public static ReportManager Reporter { get; private set; }

        public Result OnStartup(UIControlledApplication application)
        {
            if (application is null) { return Result.Failed; }

            Reporter = new ReportManager();
            Reporter.Connect();

            application.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
            application.ControlledApplication.DocumentClosing += ControlledApplication_DocumentClosing;
            application.ControlledApplication.DocumentSaved += ControlledApplication_DocumentSaved;
            application.ControlledApplication.DocumentSavedAs += ControlledApplication_DocumentSavedAs;
            application.ControlledApplication.FailuresProcessing += ControlledApplication_FailuresProcessing;
            application.DialogBoxShowing += Application_DialogBoxShowing;
            return Result.Succeeded;
        }

        private void ControlledApplication_DocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            Reporter.ActionId = ReportManager.CloseActionId;
            while (Reporter.CloseReport() == false)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            Reporter.Disconnect();
        }

        private void Application_DialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
        {
            /// TODO throw new System.NotImplementedException();
        }

        private void ControlledApplication_FailuresProcessing(object sender, FailuresProcessingEventArgs args)
        {
            ///TODO throw new System.NotImplementedException();
        }

        private void ControlledApplication_DocumentSaved(object sender, DocumentSavedEventArgs args)
        {
            Reporter.ActionId = ReportManager.SaveActionId;
            Reporter.ActionStatusReport(ActionStatus.Started);
            Reporter.SaveReport(args.Document);
            Reporter.ActionStatusReport(ActionStatus.Finished);
        }

        private void ControlledApplication_DocumentSavedAs(object sender, DocumentSavedAsEventArgs args)
        {
            Reporter.ActionId = ReportManager.SaveAsActionId;
            Reporter.ActionStatusReport(ActionStatus.Started);
            Reporter.SaveAsReport(args.Document);
            Reporter.ActionStatusReport(ActionStatus.Finished);
        }

        private void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            Reporter.ActionId = ReportManager.OpenActionId;
            Reporter.StartAction();
            Reporter.ActionStatusReport(ActionStatus.Started);
            if (Reporter.OpenReport(args.Document) == false) { return; }

            Reporter.JournalReport(args.Document);
            Reporter.ActionStatusReport(ActionStatus.Finished);
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
