using RevitAction.Action;
using RevitAction.Report;
using System.Collections.Generic;

namespace RevitJournal.Revit.Journal.Command
{
    public class DocumentSaveAction : ATaskAction, ITaskActionJournal
    {
        public ActionParameter Backup { get; set; }

        public DocumentSaveAction() : base("Save", ActionManager.SaveActionId)
        {
            Backup = ActionParameter.Bool("Delete Backup", "DeleteBackpus", true);
            Parameters.Add(Backup);
        }

        public IEnumerable<string> Commands
        {
            get { return new string[] { JournalBuilder.Build("Ribbon", "ID_REVIT_FILE_SAVE") }; }
        }
    }
}
