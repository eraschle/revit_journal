using RevitAction.Action;
using RevitJournal.Tasks;

namespace RevitJournal.Report
{
    public class TaskErrorReport : ATaskReport
    {
        public TaskErrorReport(RevitTask task, TaskUnitOfWork unitOfWork) : base(task, unitOfWork) { }

        public ITaskAction ErrorReport { get; set; } = null;

        public string ErrorMessage { get; set; } = null;
    }
}
