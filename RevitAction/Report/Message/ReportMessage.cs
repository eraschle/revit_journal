using System;

namespace RevitAction.Report.Message
{
    public class ReportMessage
    {
        public Guid ActionId { get; set; }

        public ReportKind Kind { get; set; }

        public string Message { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public Exception Exception { get; set; } = null;
        
        public bool HasException
        {
            get { return Exception != null; }
        }

        public override string ToString()
        {
            return $"{Kind}: {Message}";
        }
    }
}
