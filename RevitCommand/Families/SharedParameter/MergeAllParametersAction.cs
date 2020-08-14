using RevitJournal.Revit.Commands.Parameter;
using System;

namespace RevitCommand.Families.SharedParameter
{
    public class MergeAllParametersAction : ATaskActionCommand
    {
        public MergeAllParametersAction() : base("Merge Shared [ALL]")
        {
            Parameters.Add(new SharedFileActionParameter("File"));
            MakeChanges = true;
        }
        public override Guid AddinId
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
