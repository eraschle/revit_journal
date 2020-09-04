using System;

namespace RevitAction.Report.Message
{
    public class ReportMessage
    {
        public Guid ActionId { get; set; }

        public ReportKind Kind { get; set; }

        public string Message { get; set; }

        public int GetStatus()
        {
            switch (Kind)
            {
                case ReportKind.Open:
                    return ReportStatus.Open;
                case ReportKind.Journal:
                case ReportKind.Status:
                case ReportKind.Success:
                case ReportKind.Save:
                case ReportKind.SaveAs:
                    return ReportStatus.Running;
                case ReportKind.Error:
                    return ReportStatus.Error;
                case ReportKind.Close:
                    return ReportStatus.Finish;
                case ReportKind.Unknown:
                default:
                    return ReportStatus.Unknown;
            }
        }

        public bool IsError
        {
            get { return Kind == ReportKind.Error; }
        }
    }
}
