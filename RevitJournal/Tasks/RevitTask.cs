using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitAction.Report;
using RevitAction.Report.Message;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
using RevitJournal.Tasks.Options;
using RevitJournal.Tasks.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RevitJournal.Tasks
{
    public class RevitTask
    {
        public RevitFamily Family { get; private set; }

        public RevitFamilyFile SourceFile
        {
            get { return Family.RevitFile; }
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
