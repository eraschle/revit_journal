using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAction.Action;
using System;
using System.Collections.Generic;

namespace RevitAction.Revit
{
    public abstract class ARevitExternalCommand<TAction> : IExternalCommand where TAction : ITaskActionCommand, new()
    {
        protected UIApplication UiApplication { get; private set; }

        protected Application Application { get; private set; }

        protected UIDocument UIDocument { get; private set; }

        protected Document Document { get; private set; }

        protected TAction Action { get; private set; }

        protected ARevitExternalCommand()
        {
            Action = new TAction();
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskApp.Reporter.ActionId = Action.Id;
            TaskApp.Reporter.StatusReport($"Action started");
            if (commandData is null)
            {
                message = "No command data";
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error(message);
                return Result.Failed;
            }

            UiApplication = commandData.Application;
            Application = UiApplication.Application;
            UIDocument = commandData.Application.ActiveUIDocument;
            if (UIDocument is null)
            {
                message = "UIDocument is NULL";
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error(message);
                return Result.Failed;
            }

            Document = UIDocument.Document;
            if (Document is null)
            {
                message = "Document is NULL";
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error(message);
                return Result.Failed;
            }

            try
            {
                var result = ExecuteRevitCommand(commandData, ref message, elements);
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.StatusReport($"Successfully executed");
                return result;
            }
            catch (Exception exp)
            {
                TaskApp.Reporter.ActionId = Action.Id;
                TaskApp.Reporter.Error("Revit Execute-Method", exp);
                return Result.Failed;
            }
        }

        protected static bool HasJournal(ExternalCommandData commandData)
        {
            var journal = commandData?.JournalData;
            return journal != null && journal.Count > 0;
        }

        protected static bool HasJournal(ExternalCommandData commandData, out IDictionary<string, string> journal)
        {
            journal = commandData?.JournalData;
            return HasJournal(commandData);
        }

        protected static bool JournalKeyExist(ExternalCommandData commandData, string key, out string journalValue)
        {
            journalValue = null;
            if (HasJournal(commandData, out var journal) == false
                || journal.ContainsKey(key) == false) { return false; }

            journalValue = journal[key];
            return true;
        }

        protected abstract Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements);
    }
}
