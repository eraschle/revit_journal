using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using RevitAction.Report;

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
            application.ControlledApplication.DocumentSaved += ControlledApplication_DocumentSaved;
            application.ControlledApplication.DocumentSavedAs += ControlledApplication_DocumentSavedAs;
            application.ControlledApplication.FailuresProcessing += ControlledApplication_FailuresProcessing;
            application.DialogBoxShowing += Application_DialogBoxShowing;
            return Result.Succeeded;
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
            Reporter.SaveReport(args.Document);
        }

        private void ControlledApplication_DocumentSavedAs(object sender, DocumentSavedAsEventArgs args)
        {
            Reporter.ActionId = ReportManager.SaveAsActionId; 
            Reporter.SaveAsReport(args.Document);
        }

        private void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            Reporter.ActionId = ReportManager.OpenActionId; 
            Reporter.StartAction();
            if (Reporter.OpenReport(args.Document) == false) { return; }

            Reporter.JournalReport(args.Document);
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            Reporter.ActionId = ReportManager.CloseActionId;
            Reporter.CloseReport();
            Reporter.Disconnect();
            return Result.Succeeded;
        }
    }
}
