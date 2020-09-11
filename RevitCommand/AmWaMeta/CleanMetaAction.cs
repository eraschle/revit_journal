using RevitAction;
using RevitAction.Action;
using System;

namespace RevitCommand.AmWaMeta
{
    public class CleanMetaAction : ATaskAction, ITaskActionCommand
    {
        public CleanMetaAction() : base("Clean Metadata", new Guid("776755a1-9976-4087-9bad-94474e56b1ad"))
        {
            MakeChanges = false;
            TaskInfo = new TaskActionInfo<CleanMetaAction>(nameof(CleanMetaCommand));
        }

        public ITaskInfo TaskInfo { get; }
    }
}
