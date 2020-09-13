using DataSource;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using RevitAction.Action;
using RevitJournal.Revit;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Tasks
{
    public class RevitTask : IEquatable<RevitTask>
    {
        public RevitFamily Family { get; private set; }

        public RevitFamilyFile SourceFile
        {
            get { return Family.RevitFile; }
        }

        public RevitFamilyFile ResultFile { get; set; } = null;

        public bool HasResultFile
        {
            get { return ResultFile != null; }
        }

        public RevitFamilyFile BackupFile { get; set; } = null;

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

        internal bool HasActionById(Guid actionId, out ITaskAction action)
        {
            action = Actions.FirstOrDefault(act => act.Id == actionId);
            return action != null;
        }


        internal bool HasNextAction(Guid actionId, out ITaskAction nextAction)
        {
            nextAction = null;
            if (HasActionById(actionId, out var action))
            {
                var index = Actions.IndexOf(action) + 1;
                nextAction = index < Actions.Count ? Actions[index] : null;
            }
            return nextAction != null;
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

        public void PreExecution(BackupOptions options)
        {
            if(options is null) { throw new ArgumentNullException(nameof(options)); }

            if (options.CreateBackup)
            {
                var backupPath = options.CreateBackupFile(SourceFile);
                BackupFile = SourceFile.CopyTo<RevitFamilyFile>(backupPath, true);
            }

            foreach (var command in Actions)
            {
                command.PreTask(Family);
            }
        }

        public void DeleteBackups(TaskOptions options)
        {
            if (options is null || options.DeleteRevitBackup == false) { return; }

            SourceFile.DeleteBackups();
            if (HasResultFile)
            {
                ResultFile.DeleteBackups();
            }

            if (HasBackupFile)
            {
                BackupFile.DeleteBackups();
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RevitTask);
        }

        public bool Equals(RevitTask other)
        {
            return other != null &&
                   EqualityComparer<RevitFamily>.Default.Equals(Family, other.Family);
        }

        public override int GetHashCode()
        {
            return 548286385 + EqualityComparer<RevitFamily>.Default.GetHashCode(Family);
        }

        public static bool operator ==(RevitTask left, RevitTask right)
        {
            return EqualityComparer<RevitTask>.Default.Equals(left, right);
        }

        public static bool operator !=(RevitTask left, RevitTask right)
        {
            return !(left == right);
        }
    }
}
