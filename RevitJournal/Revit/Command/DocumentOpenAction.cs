using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitAction.Report;
using System.Collections.Generic;

namespace RevitJournal.Revit.Command
{
    public class DocumentOpenAction : ATaskAction, ITaskActionJournal
    {
        public ActionParameter Audit { get; private set; }

        public DocumentOpenAction() : base("Open File", ActionManager.OpenActionId)
        {
            Audit = ActionParameter.Bool("Audit", "Audit", false);
            Parameters.Add(Audit);
        }

        private RevitFamilyFile RevitFile { get; set; }

        public IEnumerable<string> Commands
        {
            get { return Audit.GetBoolValue() ? OpenAuditCommand : OpenCommand; }
        }

        public override bool MakeChanges
        {
            get { return Audit.GetBoolValue(); }
            protected set { }
        }

        private string[] OpenCommand
        {
            get
            {
                return new string[] {
                    JournalBuilder.Build("StartupPage", "ID_FILE_MRU_FIRST"),
                    string.Concat("Jrn.Data \"MRUFileName\" , \"", RevitFile.FullPath, "\"") };
            }
        }

        private string[] OpenAuditCommand
        {
            get
            {
                //Jrn.Command "Ribbon" , "�ffnet ein Projekt , ID_REVIT_FILE_OPEN"
                //Jrn.Data "FileOpenSubDialog", "AuditCheckBox", "True"
                //Jrn.Data "TaskDialogResult", "", "Yes", "IDYES"
                //Jrn.Data "File Name", "IDOK", "FilePath"
                return new string[] {
                    JournalBuilder.Build("Ribbon", "ID_REVIT_FILE_OPEN"),
                    string.Concat("Jrn.Data \"FileOpenSubDialog\" , \"AuditCheckBox\", \"True\""),
                    string.Concat("Jrn.Data \"TaskDialogResult\" , \"\" , \"Yes\", \"IDYES\""),
                    string.Concat("Jrn.Data \"File Name\" , \"IDOK\", \"", RevitFile.FullPath, "\"")
                };
            }
        }

        public override void PreTask(RevitFamily family)
        {
            if (family is null) { return; }

            RevitFile = family.RevitFile;
        }
    }
}


