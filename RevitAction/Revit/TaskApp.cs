using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using RevitAction.Report;

namespace RevitAction.Revit
{
    public class TaskApp : IExternalApplication
    {
        public static ReportManager Reporter { get; private set; }

        public Result OnStartup(UIControlledApplication application)
        {
            if (application is null) { return Result.Failed; }

            application.DialogBoxShowing += Application_DialogBoxShowing;
            try
            {
                Reporter = new ReportManager();
                Reporter.Connect();
                Reporter.InitialReport();
            }
            catch
            {
                return Result.Failed;
            }

            application.ControlledApplication.FailuresProcessing += ControlledApplication_FailuresProcessing;

            application.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;

            application.ControlledApplication.DocumentSaved += ControlledApplication_DocumentSaved;
            application.ControlledApplication.DocumentSavedAs += ControlledApplication_DocumentSavedAs;

            application.ControlledApplication.DocumentClosed += ControlledApplication_DocumentClosed;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            //Reporter.Disconnect();
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
                Reporter.Error("Server could not find task");
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

        private void ControlledApplication_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            Reporter.Disconnect();
        }

        #endregion

        #region Error Events

        private void Application_DialogBoxShowing(object sender, DialogBoxShowingEventArgs args)
        {
            //args.OverrideResult(-1);
            Reporter.Error(args.DialogId);
        }

        private void ControlledApplication_FailuresProcessing(object sender, FailuresProcessingEventArgs args)
        {
            var result = args.GetProcessingResult();
            if (result == FailureProcessingResult.WaitForUserInput)
            {
                Reporter.Error(result.ToString());
            }
        }

        #endregion

        #endregion

    }
}
