using System;

namespace RevitCommand.Reports
{
    public class ParameterSharedReplaceReportLine : AParameterSharedReportLine
    {
        private const string ActionName = "Replace Shared Parameter";

        public ParameterSharedReplaceReportLine() : base(ActionName) { }

        public Guid OldGuid { get; set; } = Guid.Empty;
    }
}
