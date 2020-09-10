﻿namespace RevitAction.Report.Message
{
    public class TasMessage : AReportMessage, IReportMessage
    {
        public TasMessage(ReportKind kind) : base(kind) { }

        public TasMessage(ReportKind kind, string message) : base(kind, message) { }
    }
}