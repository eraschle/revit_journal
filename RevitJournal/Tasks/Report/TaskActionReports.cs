using System.Collections.Generic;
using RevitAction.Action;
using System.Linq;
using RevitAction.Report.Message;

namespace RevitJournal.Tasks.Report
{
    public class TaskActionReports
    {
        public ITaskAction Action { get; set; }

        private readonly IList<ReportMessage> messages = new List<ReportMessage>();

        public void Add(ReportMessage message)
        {
            if (message is null || messages.Contains(message)) { return; }

            messages.Add(message);
        }

        public IEnumerable<ReportMessage> SuccessMessages
        {
            get { return GetMessages(ReportKind.Success); }
        }

        public IEnumerable<ReportMessage> ErrorMessages
        {
            get { return GetMessages(ReportKind.Error); }
        }

        public IEnumerable<ReportMessage> InfoMessages
        {
            get { return GetMessages(ReportKind.Info); }
        }

        private IEnumerable<ReportMessage> GetMessages(ReportKind kind)
        {
            return messages.Where(msg => msg.Kind == kind);
        }
    }
}
