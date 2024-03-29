﻿using Autodesk.Revit.DB;
using System;
using RevitCommand.Reports;

namespace RevitCommand.Families
{
    public class RevitFamilyManagerReport
    {
        private readonly RevitFamilyParameterManager Manager;

        public RevitFamilyManagerReport(RevitFamilyParameterManager manager)
        {
            Manager = manager;
        }

        public MessageReportLine MergeSharedParameter(ExternalDefinition definition)
        {
            if (Manager.CanMergeSharedParameter(definition, out var parameter) == false)
            {
                return null;
            }
            var reportLine = new ParameterSharedReplaceReportLine
            {
                ParameterName = definition.Name,
                ParameterGroup = LabelUtils.GetLabelFor(parameter.Definition.ParameterGroup),
                IsInstance = parameter.IsInstance,
                CurrentGuid = definition.GUID,
                OldGuid = parameter.GUID,
            };
            try
            {
                Manager.MergeSharedParameter(definition);
            }
            catch (Exception exp)
            {
                reportLine.ErrorMessage = exp.Message;
            }
            return reportLine;
        }

        public ParameterSharedAddReportLine AddSharedInstance(ExternalDefinition definition, BuiltInParameterGroup group)
        {
            var report = CreateReport(definition, group, true);
            try
            {
                Manager.AddSharedInstance(definition, group);
            }
            catch (Exception exp)
            {
                report.ErrorMessage = exp.Message;
            }
            return report;
        }

        public ParameterSharedAddReportLine AddSharedType(ExternalDefinition definition, BuiltInParameterGroup group)
        {
            var report = CreateReport(definition, group, false);
            try
            {
                Manager.AddSharedType(definition, group);
            }
            catch (Exception exp)
            {
                report.ErrorMessage = exp.Message;
            }
            return report;
        }

        private ParameterSharedAddReportLine CreateReport(ExternalDefinition definition, BuiltInParameterGroup group, bool isInstance)
        {
            return new ParameterSharedAddReportLine
            {
                ParameterName = definition.Name,
                CurrentGuid = definition.GUID,
                IsInstance = isInstance,
                ParameterGroup = LabelUtils.GetLabelFor(group)
            };
        }
    }
}
