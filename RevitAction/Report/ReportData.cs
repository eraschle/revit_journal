using RevitAction.Report.Message;
using System;

namespace RevitAction.Report
{
    [Serializable()]
    public class ReportData
    {
        public string FilePath { get; set; }

        public Guid ActionId { get; set; }

        public IReportMessage Message { get; set; }

        public bool IsSuccess()
        {
            return IsKind(ReportKind.Success);
        }

        public bool IsError()
        {
            return IsKind(ReportKind.Error);
        }

        public bool IsInfo()
        {
            return IsKind(ReportKind.Info);
        }

        private bool IsKind(ReportKind kind)
        {
            return Message != null && Message.Kind == kind;
        }
    }
}