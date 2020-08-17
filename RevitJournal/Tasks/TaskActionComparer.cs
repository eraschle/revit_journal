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
            var compareTo = 0;
            if (command != null && command.DependsOn(other))
            {
                compareTo = -1;
            }
            if (other != null && other.DependsOn(command))
            {
                compareTo = 1;
            }
            if (compareTo == 0 && command != null && other != null)
            {
                compareTo = StringUtils.Compare(command.Name, other.Name);
            }
            return compareTo;
        }
    }
}
