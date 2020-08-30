using RevitAction.Reports.Messages;
using System;

namespace RevitAction.Reports
{
    [Serializable()]
    public class ReportData
    {
        public string FilePath { get; set; }

        public Guid ActionId { get; set; }

        public IReportMessage Message { get; set; }
    }
}