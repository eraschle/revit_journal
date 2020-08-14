using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitJournal.Helper;
using System;
using System.Collections.Generic;

namespace RevitJournal.Revit.Journal.Command
{
    public class DocumentSaveAsAction : ATaskAction, ITaskActionJournal
    {
        private const string suffixFormatString = "yyyy-dd-MM";

        private readonly ActionParameter fileSuffix;
        private readonly ActionParameter saveFolder;
        private readonly ActionParameter addAtEnd;
        private readonly ActionParameter currentRoot;
        private readonly ActionParameter newRoot;

        private RevitFamilyFile familyFile;

        private readonly PathCreator pathCreator = new PathCreator();


        public DocumentSaveAsAction() : base("Save As")
        {
            AddParameter(new ActionParameterInfo("Save As Path Model", CreateSymbolicPath));

            fileSuffix = ParameterBuilder.Text("File suffix", defaultValue: GetDate());
            AddParameter(fileSuffix);

            saveFolder = ParameterBuilder.Text("Save As Folder");
            AddParameter(saveFolder);

            addAtEnd = ParameterBuilder.Bool("Add Folder At End", true);
            AddParameter(addAtEnd);

            currentRoot = ParameterBuilder.Create("Library Folder [Current]", ParameterKind.TextInfoValue);
            AddParameter(currentRoot);

            newRoot = ParameterBuilder.Create("Library Folder [New]", ParameterKind.SelectFolder);
            AddParameter(newRoot);
            IsSaveAction = true;
        }

        private string GetDate()
        {
            return DateTime.Now.ToString(suffixFormatString);
        }


        public IEnumerable<string> Commands
        {
            get
            {
                return new string[] {
                    JournalBuilder.Build("Ribbon", "ID_REVIT_SAVE_AS_FAMILY"),
                    string.Concat("Jrn.Data \"File Name\" , \"IDOK\", \"", pathCreator.CreatePath(familyFile), "\"") };
            }
        }

        private string CreateSymbolicPath()
        {
            pathCreator.FileSuffix = fileSuffix.Value; ;
            pathCreator.BackupFolder = saveFolder.Value;
            pathCreator.AddBackupAtEnd = addAtEnd.BoolValue;
            pathCreator.SetRoot(currentRoot.Value);
            pathCreator.SetNewRoot(newRoot.Value);
            return pathCreator.CreateSymbolic();
        }

        public override void PreTask(RevitFamily family)
        {
            if (family is null) { return; }

            familyFile = family.RevitFile;
        }

        public override void SetLibraryRoot(string libraryRoot)
        {
            currentRoot.Value = libraryRoot;
            newRoot.DefaultValue = libraryRoot;
        }
    }
}
