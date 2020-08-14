using RevitAction.Action;
using System;

namespace RevitCommand.RevitData
{
    public class RevitDataExportAction : ATaskActionCommand
    {
        public RevitDataExportAction() : base("Revit Data Export")
        {
            Parameters.Add(ParameterBuilder.CreateJournal("Export Directory", "DataDir", ParameterKind.SelectFolder));
        }

        public override Guid AddinId
        {
            get { return new Guid("6d6b38cf-cb70-4a73-9e2d-56fc23cd9cbf"); }
        }

        public override string Namespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(RevitDataExportCommand); }
        }
    }
}
