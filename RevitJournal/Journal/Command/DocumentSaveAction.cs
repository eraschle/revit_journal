﻿using DataSource.Model.FileSystem;
using RevitAction.Action;
using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public class DocumentSaveAction : ATaskAction, ITaskActionJournal
    {
        private readonly ActionParameter backup;

        public DocumentSaveAction() : base("Save")
        {
            backup = ParameterBuilder.Bool("Delete Backup", true);
            Parameters.Add(backup);
        }

        public IEnumerable<string> Commands
        {
            get { return new string[] { JournalBuilder.Build("Ribbon", "ID_REVIT_FILE_SAVE") }; }
        }

        public override void PostTask(RevitFamilyFile family)
        {
            if (family is null) { return; }

            foreach (var backup in family.Backups)
            {
                backup.Delete();
            }
        }
    }
}
