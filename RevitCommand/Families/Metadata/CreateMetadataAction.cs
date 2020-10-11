using DataSource.Models.FileSystem;
using RevitAction;
using RevitAction.Action;
using System;

namespace RevitCommand.Families.Metadata
{
    public class CreateMetadataAction : ATaskAction, ITaskActionCommand
    {
        public ActionParameter Library { get; set; }

        public ITaskInfo TaskInfo { get; }

        public CreateMetadataAction() : base("Create metadata", new Guid("7d3a1639-4384-4488-b34c-08f29aebac2f"))
        {
            TaskInfo = new TaskActionInfo<CreateMetadataAction>(ActionId, nameof(CreateMetadataCommand));
            Library = ActionParameter.Text("Library Path", "Library");
            Parameters.Add(Library);
        }


        public override void PreTask(RevitFamilyFile family) { }

        public override void SetLibraryRoot(string libraryRoot)
        {
            Library.Value = libraryRoot;
        }
    }
}
