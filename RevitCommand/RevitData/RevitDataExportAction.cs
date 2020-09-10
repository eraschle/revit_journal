using RevitAction.Action;
using System;

namespace RevitCommand.RevitData
{
    public class RevitDataExportAction : ATaskActionCommand
    {
        public ActionParameter ExportDirectory { get; private set; }

        public RevitDataExportAction() : base("Revit Data Export")
        {
            ExportDirectory = ParameterBuilder.CreateJournal("Export Directory", "DataDir", ParameterKind.SelectFolder);
            Parameters.Add(ExportDirectory);
        }

        public override Guid Id
        {
            get { return new Guid("6d6b38cf-cb70-4a73-9e2d-56fc23cd9cbf"); }
        }

        public override string TaskNamespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(RevitDataExportRevitCommand); }
        }
    }
}
