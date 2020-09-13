using System.Collections.Generic;
using RevitAction.Action;
using RevitAction.Report.Message;

namespace RevitJournal.Report
{
    public class TaskActionReport
    {
        private readonly Dictionary<ReportKind, List<string>> messages
            = new Dictionary<ReportKind, List<string>>();

        public void Add(ReportMessage report)
        {
            if (report is null || report.Kind == ReportKind.Error) { return; }

            if (messages.ContainsKey(report.Kind) == false)
            {
                messages.Add(report.Kind, new List<string>());
            }
            messages[report.Kind].Add(report.Message);
        }

        public ITaskAction TaskAction { get; set; }

        public IEnumerable<string> StatusReports()
        {
            return messages[ReportKind.DefaultAction];
        }

        public bool HasStatusReports()
        {
            return messages.ContainsKey(ReportKind.DefaultAction); 
        }

        public IEnumerable<string> WarningReports()
        {
            return messages[ReportKind.Warning];
        }

        public bool HasWarningReports()
        {
            return messages.ContainsKey(ReportKind.Warning);
        }
    }
}
