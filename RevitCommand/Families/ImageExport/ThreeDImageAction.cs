using RevitAction;
using RevitAction.Action;
using System;

namespace RevitCommand.Families.ImageExport
{
    public class ThreeDImageAction : ATaskAction, ITaskActionCommand
    {
        public ActionParameter Background { get; set; }

        public ThreeDImageAction() : base("3D Image Export", new Guid("d62ba092-cf93-4906-90d8-1948d2dd67c5"))
        {
            TaskInfo = new TaskActionInfo<ThreeDImageAction>(ActionId, nameof(ThreeDImageRevitCommand));
            Background = ActionParameter.Create("Background Image", "Background", ParameterKind.ImageFile);
            Parameters.Add(Background);
        }

        public ITaskInfo TaskInfo { get; }
    }
}
