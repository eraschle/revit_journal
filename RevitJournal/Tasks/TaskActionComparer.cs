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
            if (command is null && other is null) { return 0; }
            if (command is null && other != null) { return 1; }
            if (command != null && other is null) { return -1; }

            if (command is DocumentOpenAction ) { return int.MinValue; }
            if (other is DocumentOpenAction ) { return int.MaxValue; }

            if (command is DocumentSaveAction) { return int.MaxValue - 1; }
            if (other is DocumentSaveAction) { return int.MinValue + 1; }
            if (command is DocumentSaveAsAction ) { return int.MaxValue; }
            if (other is DocumentSaveAsAction) { return int.MinValue; }

            return StringUtils.Compare(command.Name, other.Name);
        }
    }
}
