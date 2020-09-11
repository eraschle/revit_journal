using System.Collections.Generic;

namespace RevitAction.Action
{
    public interface ITaskActionCommand : ITaskAction
    {
        ITaskInfo TaskInfo { get; }
    }
}
