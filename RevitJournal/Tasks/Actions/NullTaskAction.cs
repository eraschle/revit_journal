using DataSource.Model.FileSystem;
using RevitAction.Action;
using System;

namespace RevitJournal.Tasks.Actions
{
    public class NullTaskAction : ATaskAction
    {
        public NullTaskAction() : base("No Action", Guid.Empty) { }

        public override void PreTask(RevitFamily family) { }

        public override void SetLibraryRoot(string libraryRoot) { }
    }
}
