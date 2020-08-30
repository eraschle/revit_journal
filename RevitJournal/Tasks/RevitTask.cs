using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitJournal.Journal.Execution;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
using RevitJournal.Revit.Report;
using RevitJournal.Tasks;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RevitJournal.Journal
{
    public class RevitTask
    {
        public RevitFamily Family { get; private set; }

        public RevitFamilyFile SourceFile
        {
            get { return Family.RevitFile; }
        }

        public RevitProcess Process { get; internal set; }

        public TaskJournalFile JournalTask { get; private set; }

        public TaskReport Report { get; private set; }

        public IList<ITaskAction> Actions { get; private set; }

        public RevitTask(RevitFamily family)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }

            Family = family;
            Actions = new List<ITaskAction>();
            Report = new TaskReport(this);
        }

        public string Name
        {
            get { return Family.RevitFile.Name; }
        }

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
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            JournalTask = TaskJournalCreator.Create(this, options.JournalDirectory);
        }

        public void DeleteJournalProcess()
        {
            if (JournalTask is null) { return; }

            JournalTask.Delete();
        }

        internal void PreExecution(BackupOptions option)
        {
            if (option.CreateBackup)
            {
                var backupPath = option.CreateBackupFile(Family.RevitFile);
                Report.BackupFile = Family.RevitFile.CopyTo<RevitFamilyFile>(backupPath, true);
            }

            foreach (var command in Actions)
            {
                command.PreTask(Family);
            }
        }

        public void KillProcess()
        {
            if (Process is null) { return; }

            Process.KillProcess();
            Process.Dispose();
            Debug.WriteLine(Name + ": Killed Process [CancelAction]");
        }

        internal void PostExecution()
        {
            foreach (var command in Actions)
            {
                command.PostTask(Family);
            }
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
