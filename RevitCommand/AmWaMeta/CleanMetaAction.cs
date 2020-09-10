using System;

namespace RevitCommand.AmWaMeta
{
    public class CleanMetaAction : ATaskActionCommand
    {
        public CleanMetaAction() : base("Clean Metadata")
        {
            MakeChanges = false;
        }

        public override string TaskNamespace
        {
            get { return GetType().Namespace; }
        }

        public override Guid Id
        {
            get { return new Guid("776755a1-9976-4087-9bad-94474e56b1ad"); }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(CleanMetaCommand); }
        }


    }
}
