using DataSource.Model.FileSystem;
using System.Collections.Generic;

namespace RevitJournal.Journal.Command.Document
{
    public class DocumentOpenCommand : AJournalCommand
    {
        public const string AuditParameter = "Audit";

        private bool Audit = false;

        public DocumentOpenCommand() : base("Open File", false)
        {
            Parameters.Add(new CommandParameter(AuditParameter, JournalParameterType.Boolean));
        }

        public override IEnumerable<string> Commands
        {
            get { return Audit ? OpenAuditCommand : OpenCommand; }
        }

        private string[] OpenCommand
        {
            get
            {
                return new string[] {
                    JournalCommandBuilder.Build("StartupPage", "ID_FILE_MRU_FIRST"),
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
                    JournalCommandBuilder.Build("Ribbon", "ID_REVIT_FILE_OPEN"),
                    string.Concat("Jrn.Data \"FileOpenSubDialog\" , \"AuditCheckBox\", \"True\""),
                    string.Concat("Jrn.Data \"TaskDialogResult\" , \"\" , \"Yes\", \"IDYES\""),
                    string.Concat("Jrn.Data \"File Name\" , \"IDOK\", \"", RevitFile.FullPath, "\"")
                };
            }
        }

        public RevitFile RevitFile { get; set; }

        public override void PreExecutionTask(RevitFamily family)
        {
            if (bool.TryParse(Parameters[0].Value, out var isAudit))
            {
                Audit = isAudit;
            }

            RevitFile = family.RevitFile;
        }

        public override void PostExecutionTask(JournalResult result)
        {
            if (result is null) { return; }

            result.Result = RevitFile;
        }
    }
}


