﻿using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitAction.Report;
using RevitJournal.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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

        public DocumentSaveAsAction() : base("Save As", ActionManager.SaveActionId)
        {
            AddParameter(new ActionParameterInfo("Save As Path Model", CreateSymbolicPath));

            fileSuffix = ActionParameter.Text("File suffix", null, defaultValue: GetDate());
            AddParameter(fileSuffix);

            saveFolder = ActionParameter.Text("Save As Folder", null, defaultValue: GetDate());
            AddParameter(saveFolder);

            addAtEnd = ActionParameter.Bool("Add Folder At End", null, true);
            AddParameter(addAtEnd);

            currentRoot = ActionParameter.Create("Library Folder [Current]", null, ParameterKind.TextInfoValue);
            AddParameter(currentRoot);

            newRoot = ActionParameter.Create("Library Folder [New]", null, ParameterKind.SelectFolder);
            AddParameter(newRoot);
        }

        private string GetDate()
        {
            return DateTime.Now.ToString(suffixFormatString, CultureInfo.CurrentCulture);
        }


        public IEnumerable<string> Commands
        {
            get
            {
                var saveAsPath = pathCreator.CreatePath(familyFile);
                var commands = new List<string>
                {
                    JournalBuilder.Build("Ribbon", "ID_REVIT_SAVE_AS_FAMILY"),
                    string.Concat("Jrn.Data \"File Name\" , \"IDOK\", \"", saveAsPath, "\"")
                };

                if (File.Exists(saveAsPath))
                {
                    commands.Add("Jrn.Data \"TaskDialogResult\" , \"\" , \"Yes\", \"IDYES\"");
                }
                return commands.ToArray();
            }
        }

        private string CreateSymbolicPath()
        {
            pathCreator.FileSuffix = fileSuffix.Value;
            pathCreator.BackupFolder = saveFolder.Value;
            pathCreator.AddBackupAtEnd = addAtEnd.GetBoolValue();
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
