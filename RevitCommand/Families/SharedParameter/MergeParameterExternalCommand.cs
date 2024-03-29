﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitCommand.Reports;
using System;

namespace RevitCommand.Families.SharedParameter
{

    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MergeParameterExternalCommand : AParameterExternalCommand
    {
        private const string NoSharedJournalKey = "No Key for Shared Parameters found";
        private const string NoSharedParanmeter = "No Shared Parameter File found";
        private const string SearchSharedParanmeter = "Search Shared Parameters";

        public MergeParameterExternalCommand()
            : base(MergeParameterCommandData.KeySharedFile) { }

        protected override Result ManageSharedParameter(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var parametersKey = MergeParameterCommandData.KeySharedParameters;
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
            var addIfNotKey = MergeParameterCommandData.KeyAddifNot;
            if (JournalKeyExist(commandData, addIfNotKey, out var addIfNotValue))
            {
                if (bool.TryParse(addIfNotValue, out var addIfNot))
                {
                    addIfNotExist = addIfNot;
                }
            }

            var addIfNotInstanceExist = false;
            var addIfNotInstanceKey = MergeParameterCommandData.KeyAddifNotIsInstance;
            if (JournalKeyExist(commandData, addIfNotInstanceKey, out var addIfNotInstanceValue))
            {
                if (bool.TryParse(addIfNotInstanceValue, out var addIfNotInstance))
                {
                    addIfNotInstanceExist = addIfNotInstance;
                }
            }

            var addIfNotGroup = BuiltInParameterGroup.PG_DATA;
            var addIfNotGroupKey = MergeParameterCommandData.KeyAddifNotParameterGroup;
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

            var filePath = commandData.JournalData[MergeParameterCommandData.KeySharedFile];
            var sharedManager = new SharedParameterManager(Application, filePath);
            var managerFamily = new RevitFamilyParameterManager(Document);
            var managerReport = new RevitFamilyManagerReport(managerFamily);
            var report = new Report(AFile.Create<RevitFile>(Document.PathName));
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
                    if (addIfNotInstanceExist)
                    {
                        line = managerReport.AddSharedInstance(definition, addIfNotGroup);
                    }
                    else
                    {
                        line = managerReport.AddSharedType(definition, addIfNotGroup);
                    }
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
