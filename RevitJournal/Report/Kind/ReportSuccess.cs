using RevitJournal.Tasks;
using System.Collections.Generic;

namespace RevitJournal.Report
{
    public class ReportSuccess : ATaskReport
    {
        public ReportSuccess(TaskUnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<string> SuccessReport { get; } = new List<string>();


    }
}
