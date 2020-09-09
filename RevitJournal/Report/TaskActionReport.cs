using System.Collections.Generic;
using RevitAction.Report.Message;

namespace RevitJournal.Report
{
    public class TaskActionReport
    {
        private readonly Dictionary<ReportKind, List<string>> messages
            = new Dictionary<ReportKind, List<string>>();

        public void Add(ReportMessage report)
        {
            if (report is null) { return; }

            if (messages.ContainsKey(report.Kind) == false)
            {
                messages.Add(report.Kind, new List<string>());
            }
            messages[report.Kind].Add(report.Message);
        }

        public IEnumerable<string> StatusReports
        {
            get { return messages[ReportKind.Status]; }
        }

        public bool HasStatusReports
        {
            get { return messages.ContainsKey(ReportKind.Status); }
        }
    }
}
