using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitAction.Report;
using System.Collections.Generic;

namespace RevitJournal.Revit.Command
{
    public class DocumentSaveAction : ATaskAction, ITaskActionJournal
    {
        public DocumentSaveAction() : base("Save", ActionManager.SaveActionId) { }

        public IEnumerable<string> Commands
        {
            get { return new string[] { JournalBuilder.Build("Ribbon", "ID_REVIT_FILE_SAVE") }; }
        }

        public override void PreTask(RevitFamily family) { }

        public override void SetLibraryRoot(string libraryRoot) { }
    }
}
