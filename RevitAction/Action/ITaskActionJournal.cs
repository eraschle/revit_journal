using System.Collections.Generic;

namespace RevitAction.Action
{
    public interface ITaskActionJournal : ITaskAction
    {
        IEnumerable<string> Commands { get; }
    }
}
