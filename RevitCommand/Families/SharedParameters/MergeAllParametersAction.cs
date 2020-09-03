using System;

namespace RevitCommand.Families.SharedParameters
{
    public class MergeAllParametersAction : AParametersAction
    {
        public MergeAllParametersAction() : base("Merge Shared [ALL]") { }

        public override Guid Id
        {
            get { return new Guid("af072261-088e-42d3-bf5e-39fc99ea5736"); }
        }

        public override string Namespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(MergeAllParametersRevitCommand); }
        }
    }
}
