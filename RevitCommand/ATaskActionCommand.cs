using RevitAction;
using RevitAction.Action;
using System;

namespace RevitCommand
{
    public abstract class ATaskActionCommand<TAction, TCommand> : ATaskAction, ITaskActionCommand
        where TAction : ITaskAction
        where TCommand : class
    {
        protected ATaskActionCommand(string name, Guid actionId) : base(name, actionId)
        {
            TaskInfo = new TaskActionInfo<TAction, TCommand> { Id = actionId };
        }

        public ITaskInfo TaskInfo { get; }
    }
}