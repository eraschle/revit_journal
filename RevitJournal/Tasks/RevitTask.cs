using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitJournal.Journal.Execution;
using RevitJournal.Tasks;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.Generic;

namespace RevitJournal.Journal
{
    public class RevitTask
    {
        public RevitFamily Family { get; private set; }

        public RevitFamilyFile RevitFile { get { return Family.RevitFile; } }

        public ProcessJournalFile JournalProcess { get; private set; }

        public IList<ITaskAction> Actions { get; private set; }

        public RevitTask(RevitFamily family)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }

            Family = family;
            Actions = new List<ITaskAction>();
        }

        public string Name { get { return Family.RevitFile.Name; } }

        public void ClearActions()
        {
            Actions.Clear();
        }

        public void AddAction(ITaskAction command)
        {
            if (Actions.Contains(command)) { return; }

            Actions.Add(command);
        }

        public bool HasCommands(out ICollection<ITaskActionCommand> actionCommands)
        {
            actionCommands = new List<ITaskActionCommand>();
            foreach (var command in Actions)
            {
                if (!(command is ITaskActionCommand actionCommand)) { continue; }

                actionCommands.Add(actionCommand);
            }
            return actionCommands.Count > 0;
        }

        public void CreateJournalProcess(TaskOptions options)
        {
            if(options is null) { throw new ArgumentNullException(nameof(options)); }

            JournalProcess = ProcessJournalCreator.Create(this, options.JournalDirectory);
        }

        public void DeleteJournalProcess()
        {
            if (JournalProcess is null) { return; }

            JournalProcess.Delete();
        }

        internal void PreExecution(BackupOptions option)
        {
            if (option.CreateBackup)
            {
                var revitFile = Family.RevitFile;
                var backupPath = option.CreateBackupFile(revitFile);
                revitFile.CopyTo<RevitFamilyFile>(backupPath, true);
            }

            foreach (var command in Actions)
            {
                command.PreTask(Family);
            }
        }

        internal JournalResult PostExecution(JournalResult result)
        {
            foreach (var command in Actions)
            {
                command.PostTask(Family);
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is RevitTask task &&
                   EqualityComparer<RevitFamilyFile>.Default.Equals(Family.RevitFile, task.Family.RevitFile);
        }

        public override int GetHashCode()
        {
            return 1472110217 + EqualityComparer<RevitFamilyFile>.Default.GetHashCode(Family.RevitFile);
        }
    }
}
