using DataSource.Model.FileSystem;
using RevitAction;
using RevitAction.Action;
using RevitCommand.AmWaMeta;
using System;

namespace RevitCommand.JournalCommand
{
    public class PurgeUnusedAction : ATaskAction, ITaskActionCommand
    {
        public ActionParameter RevitCommand { get; set; }

        public ActionParameter Repetitions { get; set; }

        public PurgeUnusedAction() : base("Purge unused", new Guid("eb359d75-7434-4ef8-b2ef-1ba68d71946d"))
        {
            MakeChanges = true;
            TaskInfo = new TaskActionInfo<CleanMetaAction>(ActionId, nameof(PurgeUnusedCommand));

            RevitCommand = ActionParameter.Create("Revit Command", "CommandId", ParameterKind.Hidden, "PurgeUnused");
            Parameters.Add(RevitCommand);

            Repetitions = ActionParameter.Text("Repetitions", "Repetitions", "3", "3");
            Parameters.Add(Repetitions);

            /// TODO Check Dialog ID, Button ID and Command ID in jorunal file
            DialogHandlers.Add(new DialogHandler(ActionId, "ButtonId", DialogHandler.YES));
        }

        public ITaskInfo TaskInfo { get; }

        public override void PreTask(RevitFamily family) { }

        public override void SetLibraryRoot(string libraryRoot) { }
    }
}
