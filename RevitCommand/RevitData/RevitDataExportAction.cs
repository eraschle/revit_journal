using RevitAction.Action;
using System;
using System.Collections.Generic;

namespace RevitCommand.RevitData
{
    public class RevitDataExportAction : ATaskActionCommand<RevitDataExportAction, RevitDataExportCommand>
    {
        public ActionParameter ExportDirectory { get; private set; }

        public RevitDataExportAction() : base("Revit Data Export", new Guid("6d6b38cf-cb70-4a73-9e2d-56fc23cd9cbf"))
        {
            ExportDirectory = ActionParameter.Create("Export Directory", "ExportDir", ParameterKind.SelectFolder);
            Parameters.Add(ExportDirectory);
        }
    }
}
