using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using RevitAction.Report;
using System;
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
            return Result.Succeeded;
        }

        private void ControlledApplication_DocumentSaved(object sender, DocumentSavedEventArgs args)
        {
            Reporter.SaveAction(args.Document.PathName);
        }

        private void ControlledApplication_DocumentSavedAs(object sender, DocumentSavedAsEventArgs args)
        {
            Reporter.SaveAsAction(args.Document.PathName);
        }

        private void ControlledApplication_DocumentClosing(object sender, DocumentClosingEventArgs args)
        {
            Reporter.CloseAction(args.Document.PathName);
        }

        private void ControlledApplication_DocumentOpened(object sender, DocumentOpenedEventArgs args)
        {
            Reporter.StartAction();

            Reporter.Journal = args.Document.Application.RecordingJournalFilename;
            Reporter.OpenAction(args.Document.PathName);
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            Reporter.Disconnect();
            return Result.Succeeded;
        }
    }
}
