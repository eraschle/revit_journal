using RevitAction.Action;
using System;

namespace RevitJournal.Tasks.Actions
{
    public class NullTaskAction : ATaskAction
    {
        public NullTaskAction() : base("No Action") { }

        public override Guid Id => Guid.Empty;
    }
}
