using RevitJournal.Tasks;
using System.Collections.Generic;

namespace RevitJournal.Report
{
    public class TaskSuccessReport : ATaskReport
    {
        public TaskSuccessReport(RevitTask task, TaskUnitOfWork unitOfWork) : base(task, unitOfWork) { }

        public List<string> SuccessReport { get; } = new List<string>();


    }
}
