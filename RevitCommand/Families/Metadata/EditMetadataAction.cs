using RevitAction;
using RevitAction.Action;
using System;

namespace RevitCommand.Families.Metadata
{
    public class EditMetadataAction : ATaskAction, ITaskActionCommand
    {
        public ActionParameter Library { get; set; }

        public ITaskInfo TaskInfo { get; }

        public EditMetadataAction() : base("Change Metadata", new Guid("318cfd92-27ee-47a8-bb0f-840a9ff0b081"))
        {
            TaskInfo = new TaskActionInfo<EditMetadataAction>(nameof(EditMetadataCommand));
            Library = ActionParameter.Text("Library Path", "Library");
            Parameters.Add(Library);
        }

        public override void SetLibraryRoot(string libraryRoot)
        {
            Library.Value = libraryRoot;
        }
    }
}
