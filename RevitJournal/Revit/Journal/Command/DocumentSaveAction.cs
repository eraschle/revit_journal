using RevitAction.Action;
using RevitAction.Report;
using System.Collections.Generic;

namespace RevitJournal.Revit.Journal.Command
{
    public class DocumentSaveAction : ATaskAction, ITaskActionJournal
    {
        public DocumentSaveAction() : base("Save", ActionManager.SaveActionId) { }

        public IEnumerable<string> Commands
        {
            get { return new string[] { JournalBuilder.Build("Ribbon", "ID_REVIT_FILE_SAVE") }; }
        }
    }
}
