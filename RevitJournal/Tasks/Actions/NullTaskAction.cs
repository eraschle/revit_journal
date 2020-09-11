using RevitAction.Action;
using System;

namespace RevitJournal.Tasks.Actions
{
    public class NullTaskAction : ATaskAction
    {
        public NullTaskAction() : base("No Action", Guid.Empty) { }
    }
}
