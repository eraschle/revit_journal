using RevitJournal.Tasks;
using System.Collections.Generic;

namespace RevitJournal.Report
{
    public class ReportSuccess : ATaskReport
    {
        public ReportSuccess(RevitTask task) : base(task) { }

        public List<string> SuccessReport { get; } = new List<string>();


    }
}
