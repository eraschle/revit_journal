using DataSource.Model.FileSystem;
using RevitAction.Action;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Tasks
{
    public class RevitTask
    {
        public RevitFamily Family { get; private set; }

        public RevitFamilyFile SourceFile
        {
            get { return Family.RevitFile; }
        }

        public RevitFamilyFile ResultFile { get; set; }

        public bool HasResultFile
        {
            get { return ResultFile != null; }
        }

        public RevitFamilyFile BackupFile { get; set; }

        public bool HasBackupFile
        {
            get { return BackupFile != null; }
        }

        public IList<ITaskAction> Actions { get; private set; }

        public RevitTask(RevitFamily family)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }

            Family = family;
            Actions = new List<ITaskAction>();
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

        internal bool HasActionById(Guid actionId, out ITaskAction action)
        {
            action = Actions.FirstOrDefault(act => act.Id == actionId);
            return action != null;
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
