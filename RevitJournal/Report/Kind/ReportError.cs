using RevitAction.Action;
using RevitJournal.Tasks;

namespace RevitJournal.Report
{
    public class ReportError : ATaskReport
    {
        public ReportError(RevitTask task) : base(task) { }

        public ITaskAction ErrorReport { get; set; } = null;

        public string ErrorMessage { get; set; } = null;
    }
}
