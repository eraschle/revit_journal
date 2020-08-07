using System;

namespace RevitCommand.Reports
{
    public class AParameterSharedReportLine : AParameterReportLine
    {
        public AParameterSharedReportLine(string action) : base(action) { }

        public Guid CurrentGuid { get; set; } = Guid.Empty;
    }
}
