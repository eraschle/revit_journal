using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitCommand.Reports;
using RevitJournal.Journal.Command;
using System;

namespace RevitCommand.Families.SharedParameter
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MergeSelectParametersRevitCommand : AParameterRevitCommand<MergeSelectParameterAction>
    {
        private const string NoSharedJournalKey = "No Key for Shared Parameters found";
        private const string NoSharedParanmeter = "No Shared Parameter File found";
        private const string SearchSharedParanmeter = "Search Shared Parameters";

        public MergeSelectParametersRevitCommand() : base() { }

        protected override Result ManageSharedParameter(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var parametersKey = Action.SharedParameters.JournalKey;
            if (JournalKeyExist(commandData, parametersKey, out var parameterValue) == false)
            {
                message = NoSharedJournalKey;
                return Result.Failed;
            }

            var ifcParameters = ParameterListConverter.GetList(parameterValue);
            if (ifcParameters.Count == 0)
            {
                message = NoSharedParanmeter;
                return Result.Failed;
            }

            var addIfNotExist = false;
            var addIfNotKey = Action.AddIfNotExists.JournalKey;
            if (JournalKeyExist(commandData, addIfNotKey, out var addIfNotValue))
            {
                if (bool.TryParse(addIfNotValue, out var addIfNot))
                {
                    addIfNotExist = addIfNot;
                }
            }

            var addIfNotInstanceExist = false;
            var addIfNotInstanceKey = Action.IsInstance.JournalKey;
            if (JournalKeyExist(commandData, addIfNotInstanceKey, out var addIfNotInstanceValue))
            {
                if (bool.TryParse(addIfNotInstanceValue, out var addIfNotInstance))
                {
                    addIfNotInstanceExist = addIfNotInstance;
                }
            }

            var addIfNotGroup = BuiltInParameterGroup.PG_DATA;
            var addIfNotGroupKey = Action.ParameterGroups.JournalKey;
            if (JournalKeyExist(commandData, addIfNotGroupKey, out var addIfNotGroupValue))
            {
                foreach (BuiltInParameterGroup parameterGroup in Enum.GetValues(typeof(BuiltInParameterGroup)))
                {
                    var groupName = LabelUtils.GetLabelFor(parameterGroup);
                    if (groupName.Equals(addIfNotGroupValue, StringComparison.CurrentCulture) == false) { continue; }

                    addIfNotGroup = parameterGroup;
                    break;
                }
            }

            var filePath = commandData.JournalData[SharedFileJournalKey];
            var sharedManager = new SharedParameterManager(Application, filePath);
            var managerFamily = new RevitFamilyParameterManager(Document);
            var managerReport = new RevitFamilyManagerReport(managerFamily);
            var report = new Report(AFile.Create<RevitFamilyFile>(Document.PathName));
            foreach (var ifcParameter in ifcParameters)
            {
                MessageReportLine line = null;
                if (sharedManager.HasSharedParameter(ifcParameter, out var definition) == false)
                {
                    line = new MessageReportLine(SearchSharedParanmeter)
                    {
                        ErrorMessage = $"Shared Parameter [{ifcParameter}] does not exist in file {filePath}",
                    };
                }
                else if (managerFamily.HasParameterByName(ifcParameter))
                {
                    line = managerReport.MergeSharedParameter(definition);
                }
                else if (addIfNotExist)
                {
                    line = addIfNotInstanceExist
                        ? managerReport.AddSharedInstance(definition, addIfNotGroup)
                        : managerReport.AddSharedType(definition, addIfNotGroup);
                }
                report.AddLine(line);
            }

            if (report.IsEmpty == false)
            {
                report.Write(new string[] { "merge", "ifc", "parameter" });
            }
            return Result.Succeeded;
        }
    }
}
