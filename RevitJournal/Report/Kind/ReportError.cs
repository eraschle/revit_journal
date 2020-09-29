using RevitAction.Action;
using RevitJournal.Tasks;

namespace RevitJournal.Report
{
    public class ReportError : ATaskReport
    {
        public ReportError(TaskUnitOfWork unitOfWork) : base(unitOfWork) { }

        public ITaskAction ErrorReport { get; set; } = null;

        public string ErrorMessage { get; set; } = null;
    }
}
