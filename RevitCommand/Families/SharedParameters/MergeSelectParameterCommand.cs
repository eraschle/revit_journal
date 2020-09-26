using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitAction.Action.Parameter;
using RevitAction.Revit;
using RevitCommand.Reports;
using System;
using System.IO;
using Utilities.System;

namespace RevitCommand.Families.SharedParameters
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MergeSelectParameterCommand : ARevitActionCommand<MergeSelectParameterAction>
    {
        private const string NoSharedParanmeter = "No Shared Parameter File found";
        private const string SearchSharedParanmeter = "Search Shared Parameters";

        public MergeSelectParameterCommand() : base() { }

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (File.Exists(Action.SharedFile.Value) == false)
            {
#if DEBUG
                Action.SharedFile.Value = @"C:\Users\rasc\OneDrive - Amstein + Walthert AG\workspace\amwa\Shared Parameter\AWH_Shared_Parameter.txt";
#else
                errorMessage = $"Shared Parameter file does NOT exist: {filePath}";
                return Result.Failed;
#endif
            }

            if (Action.SharedParameters.ParameterNames.Count == 0)
            {
                message = NoSharedParanmeter;
                return Result.Succeeded;
            }


            var sharedManager = new SharedParameterManager(Application, Action.SharedFile.Value);
            var managerFamily = new RevitFamilyParameterManager(Document);
            var managerReport = new RevitFamilyManagerReport(managerFamily);

            var root = PathFactory.Instance.CreateRoot(Action.RootDirectory.Value);
            var reportFile = PathFactory.Instance.Create<RevitFamilyFile>(Document.PathName);
            reportFile.AddSuffixes("merge", "shared", "parameter");

            var report = new Report(reportFile);
            foreach (var ifcParameter in Action.SharedParameters.ParameterNames)
            {
                MessageReportLine line = null;
                if (sharedManager.HasSharedParameter(ifcParameter, out var definition) == false)
                {
                    line = new MessageReportLine(SearchSharedParanmeter)
                    {
                        ErrorMessage = $"Shared Parameter [{ifcParameter}] does not exist in file {Action.SharedFile.Value}",
                    };
                }
                else if (managerFamily.HasParameterByName(ifcParameter))
                {
                    line = managerReport.MergeSharedParameter(definition);
                }
                else if (Action.AddIfNotExists.GetBoolValue())
                {
                    var paramGroup = GetParameterGroup();
                    line = Action.IsInstance.GetBoolValue()
                        ? managerReport.AddSharedInstance(definition, paramGroup)
                        : managerReport.AddSharedType(definition, paramGroup);
                }
                report.AddLine(line);
            }

            if (report.IsEmpty == false)
            {
                report.Write();
            }
            return Result.Succeeded;
        }

        private BuiltInParameterGroup GetParameterGroup()
        {
            var paramGroup = BuiltInParameterGroup.PG_DATA;
            var paramGroupName = Action.ParameterGroup.Value;
            foreach (BuiltInParameterGroup parameterGroup in Enum.GetValues(typeof(BuiltInParameterGroup)))
            {
                var groupName = LabelUtils.GetLabelFor(parameterGroup);
                if (StringUtils.Equals(groupName, paramGroupName) == false) { continue; }

                paramGroup = parameterGroup;
                break;
            }

            return paramGroup;
        }
    }
}
