using RevitAction;
using RevitAction.Action;
using System;

namespace RevitCommand.Families.ImageExport
{
    public class TwoDImageAction : ATaskAction, ITaskActionCommand
    {
        public TwoDImageAction() : base("2D Image Export", new Guid("52f427dd-6db4-43bd-b4d2-c7327ae274fa"))
        {
            TaskInfo = new TaskActionInfo<TwoDImageAction>(ActionId, nameof(TwoDImageRevitCommand));
        }

        public ITaskInfo TaskInfo { get; }
    }
}
