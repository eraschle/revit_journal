using DataSource.Model.FileSystem;
using RevitAction.Action;
using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public class DocumentOpenAction : ATaskAction, ITaskActionJournal
    {
        public const string AUDITPARAMETER = "Audit";

        private readonly ActionParameter audit;

        public DocumentOpenAction() : base("Open File")
        {
            audit = ParameterBuilder.Bool(AUDITPARAMETER, false);
            Parameters.Add(audit);
        }

        public IEnumerable<string> Commands
        {
            get { return audit.BoolValue ? OpenAuditCommand : OpenCommand; }
        }

        private string[] OpenCommand
        {
            get
            {
                return new string[] {
                    JournalBuilder.Build("StartupPage", "ID_FILE_MRU_FIRST"),
                    string.Concat("Jrn.Data \"MRUFileName\" , \"", FamilyFile.FullPath, "\"") };
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
                    string.Concat("Jrn.Data \"File Name\" , \"IDOK\", \"", FamilyFile.FullPath, "\"")
                };
            }
        }

        public RevitFamilyFile FamilyFile { get; set; }

        public override void PreTask(RevitFamily family)
        {
            if (family is null) { return; }

            FamilyFile = family.RevitFile;
        }
    }
}


