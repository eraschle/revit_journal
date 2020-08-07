using RevitMetaDaten.Model;
using RevitMetaDaten.Journal;
using RevitMetaDaten.Journal.Command;
using System;
using System.Collections.Generic;
using RevitMetaDaten.Revit;

namespace DataSource.Metadata
{
    public class MetaDataManagerRepair
    {
        private readonly JournalTaskManager TaskManager;

        public MetaDataManagerRepair(JournalTaskManager taskManager)
        {
            TaskManager = taskManager;
        }

        public void AddTask(RevitApp revitApp, RevitFamilyContainer container)
        {
            if (container is null) { throw new ArgumentNullException(nameof(container)); }

            var journalTask = new JournalTask(revitApp, container.RevitFile);
            var revitFile = container.RevitFile;
            var saveAs = revitFile.ChangeFileName<RevitFile>(revitFile.Name + "_REPAIRED");
            journalTask.AddCommand(new DocumentSaveAsJournalCommand(saveAs));
            TaskManager.AddTask(journalTask);
        }

        public IEnumerable<JournalTask> JournalTasks
        {
            get { return TaskManager.JournalTasks; }
        }

        public JournalTaskResult ExecuteJournalTask(JournalTask journalTask)
        {
            var taskResult = TaskManager.ExecuteJournalTask(journalTask);
            RenameRepairedRevitFile(journalTask);
            return taskResult;
        }

        private void RenameRepairedRevitFile(JournalTask journalTask)
        {
            if(journalTask is null) { return; }

            var open = journalTask.GetCommandBy(RevitFileCommand.Open).RevitFile;
            var saveAs = journalTask.GetCommandBy(RevitFileCommand.SaveAs).RevitFile;
            saveAs.CopyTo<RevitFile>(open, true);
            saveAs.Delete();
        }
    }
}
