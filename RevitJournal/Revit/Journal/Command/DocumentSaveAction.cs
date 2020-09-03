using RevitAction.Action;
using RevitAction.Report;
using System;
using System.Collections.Generic;

namespace RevitJournal.Revit.Journal.Command
{
    public class DocumentSaveAction : ATaskAction, ITaskActionJournal
    {
        public ActionParameter Backup { get; set; }

        public DocumentSaveAction() : base("Save")
        {
            Backup = ParameterBuilder.Bool("Delete Backup", true);
            Parameters.Add(Backup);
        }

        public IEnumerable<string> Commands
        {
            get { return new string[] { JournalBuilder.Build("Ribbon", "ID_REVIT_FILE_SAVE") }; }
        }
        public override Guid Id
        {
            get { return ReportManager.SaveActionId; }
        }

        public override void PostTask(ITaskReport report)
        {
            if (report is null) { return; }

            foreach (var backup in report.SourceFile.Backups)
            {
                backup.Delete();
            }
        }
    }
}
