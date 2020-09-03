using System;

namespace RevitAction.Report.Message
{
    public class ReportMessage
    {
        public string FilePath { get; set; }

        public string Journal { get; set; }

        public Guid ActionId { get; set; }

        public ReportKind Kind { get; set; }

        public string Message { get; set; }

        public int GetStatus()
        {
            return IsError
                ? ReportStatus.Error
                : ReportStatus.Running;
        }

        public bool IsError
        {
            get { return Kind == ReportKind.Error; }
        }
    }
}
