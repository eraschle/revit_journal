using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAction.Action;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RevitAction.Revit
{
    public abstract class ARevitActionCommand<TAction> : IExternalCommand
        where TAction : ITaskActionCommand, new()
    {
        protected UIApplication UiApplication { get; private set; }

        protected Application Application { get; private set; }

        protected UIDocument UIDocument { get; private set; }

        protected Document Document { get; private set; }

        protected TAction Action { get; private set; }

        protected ARevitActionCommand()
        {
            Action = new TAction();
        }

        public Result Execute(ExternalCommandData commandData, ref string errorMessage, ElementSet elements)
        {
            TaskApp.Reporter.ActionId = Action.Id;
            TaskApp.Reporter.StatusReport($"Action started");
            if (commandData is null)
            {
                errorMessage = "No command data";
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error(errorMessage);
                return Result.Failed;
            }

            UiApplication = commandData.Application;
            Application = UiApplication.Application;
            UIDocument = commandData.Application.ActiveUIDocument;
            if (UIDocument is null)
            {
                errorMessage = "UIDocument is NULL";
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error(errorMessage);
                return Result.Failed;
            }

            Document = UIDocument.Document;
            if (Document is null)
            {
                errorMessage = "Document is NULL";
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error(errorMessage);
                return Result.Failed;
            }

            SetJournalData(commandData.JournalData);

            try
            {
                string message = null;
                errorMessage = null;
                var result = ExecuteRevitCommand(ref message, ref errorMessage);
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.StatusReport(message ?? "Successfully executed");
                return result;
            }
            catch (Exception exp)
            {
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error(errorMessage ?? "Revit Execute-Method", exp);
                return Result.Failed;
            }
        }

        private void SetJournalData(IDictionary<string, string> journalData)
        {
            foreach (var parameter in Action.Parameters)
            {
                if (parameter.IsJournalParameter && journalData.ContainsKey(parameter.JournalKey))
                {
                    parameter.SetJournalValue(journalData[parameter.JournalKey]);
                }
            }
        }

        protected abstract Result ExecuteRevitCommand(ref string message, ref string errorMessage);
    }
}
