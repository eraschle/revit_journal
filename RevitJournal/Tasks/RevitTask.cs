using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitJournal.Journal.Execution;
using System;
using System.Collections.Generic;

namespace RevitJournal.Journal
{
    public class RevitTask
    {
        public RevitFamily Family { get; private set; }

        public RevitFamilyFile RevitFile { get { return Family.RevitFile; } }

        public JournalOption TaskOption { get; private set; }

        public JournalProcessFile JournalProcess { get; private set; }

        public IList<ITaskAction> Actions { get; private set; }

        public RevitTask(RevitFamily family, JournalOption taskOption)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }
            if (taskOption is null) { throw new ArgumentNullException(nameof(taskOption)); }

            Family = family;
            TaskOption = taskOption;
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

        public void CreateJournalProcess(string journalDirectory)
        {
            JournalProcess = JournalProcessCreator.Create(this, journalDirectory);
        }

        public void DeleteJournalProcess()
        {
            if (JournalProcess is null) { return; }

            JournalProcess.Delete();
        }

        internal void PreExecution(JournalOption option)
        {
            if (option.BackupRevitFile)
            {
                var revitFile = Family.RevitFile;
                revitFile.CopyTo<RevitFamilyFile>(option.GetBackupFile(revitFile), true);
            }

            foreach (var command in Actions)
            {
                command.PreTask(Family.RevitFile);
            }
        }

        internal JournalResult PostExecution(JournalResult result)
        {
            foreach (var command in Actions)
            {
                command.PostTask(Family.RevitFile);
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
