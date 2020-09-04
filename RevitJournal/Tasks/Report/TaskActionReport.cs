using System.Collections.Generic;
using RevitAction.Report.Message;

namespace RevitJournal.Tasks.Report
{
    public class TaskActionReport
    {
        private readonly Dictionary<ReportKind, List<string>> messages
            = new Dictionary<ReportKind, List<string>>();

        public string Status { get; set; }

        public void Add(ReportMessage report)
        {
            if (report is null) { return; }

            if (messages.ContainsKey(report.Kind) == false)
            {
                messages.Add(report.Kind, new List<string>());
            }
            messages[report.Kind].Add(report.Message);
        }

        public IEnumerable<string> SuccessMessages
        {
            get { return messages[ReportKind.Success]; }
        }

        public IEnumerable<string> ErrorMessages
        {
            get { return messages[ReportKind.Error]; }
        }
    }
}
