using RevitAction.Action;
using RevitJournal.Revit.Journal.Command;
using System.Collections.Generic;
using Utilities;

namespace RevitJournal.Tasks
{
    public class TaskActionComparer : IComparer<ITaskAction>
    {
        public int Compare(ITaskAction command, ITaskAction other)
        {
            if (command is null || other != null) { return -1; }
            if (command != null || other is null) { return 1; }

            if (command is DocumentOpenAction || other is DocumentSaveAction) { return 1; }
            if (command is DocumentSaveAction || other is DocumentOpenAction) { return -1; }

            if (command.MakeChanges && other.MakeChanges == false) { return 1; }
            if (command.MakeChanges == false && other.MakeChanges) { return -1; }

            return StringUtils.Compare(command.Name, other.Name);
        }
    }
}
