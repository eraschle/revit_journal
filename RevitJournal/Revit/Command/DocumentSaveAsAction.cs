using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitAction.Report;
using RevitJournal.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using Utilities.System;

namespace RevitJournal.Revit.Command
{
    public class DocumentSaveAsAction : ATaskAction, ITaskActionJournal
    {
        private readonly ActionParameter fileSuffix;
        private readonly ActionParameter saveFolder;
        private readonly ActionParameter addAtEnd;
        private readonly ActionParameter currentRoot;
        private readonly ActionParameter newRoot;

        private RevitFamilyFile saveAsPath;

        private readonly PathCreator pathCreator = new PathCreator(PathFactory.Instance);

        public DocumentSaveAsAction() : base("Save As", ActionManager.SaveActionId)
        {
            AddParameter(new ActionParameterInfo("Save As Path Model", CreateSymbolicPath));

            fileSuffix = ActionParameter.Text("File suffix", null, defaultValue: DateUtils.GetPathDate());
            AddParameter(fileSuffix);

            saveFolder = ActionParameter.Text("Save As Folder", null, defaultValue: DateUtils.GetPathDate());
            AddParameter(saveFolder);

            addAtEnd = ActionParameter.Bool("Add Folder At End", null, true);
            AddParameter(addAtEnd);

            currentRoot = ActionParameter.Create("Library Folder [Current]", null, ParameterKind.TextInfoValue);
            AddParameter(currentRoot);

            newRoot = ActionParameter.Create("Library Folder [New]", null, ParameterKind.SelectFolder);
            AddParameter(newRoot);

            DialogHandlers.Add(new DialogHandler(ActionId, "TaskDialog_Replace_Existing_File", DialogHandler.YES));
            DialogHandlers.Add(new DialogHandler(ActionId, "TaskDialog_Newer_File_Exists", DialogHandler.YES));
        }

        public IEnumerable<string> Commands
        {
            get
            {
                return new string[]
                {
                    JournalBuilder.Build("Ribbon", "ID_REVIT_SAVE_AS_FAMILY"),
                    string.Concat("Jrn.Data \"File Name\" , \"IDOK\", \"", saveAsPath.FullPath, "\"")
                };
            }
        }

        private string CreateSymbolicPath()
        {
            pathCreator.FileSuffix = fileSuffix.Value;
            pathCreator.NewFolder = saveFolder.Value;
            pathCreator.AddBackupAtEnd = addAtEnd.GetBoolValue();
            pathCreator.RootPath = currentRoot.Value;
            pathCreator.NewRootPath = newRoot.Value;
            return pathCreator.CreateSymbolic();
        }

        public override void PreTask(RevitFamily family)
        {
            if (family is null) { return; }

            saveAsPath = pathCreator.CreatePath(family.RevitFile);
            saveAsPath?.Delete();
        }

        public override void SetLibraryRoot(string libraryRoot)
        {
            currentRoot.Value = libraryRoot;
            newRoot.DefaultValue = libraryRoot;
        }
    }
}
