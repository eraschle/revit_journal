using RevitAction.Action;

namespace RevitAction.Report
{
    public class SuccessReport : AReport
    {
        public void Add(ITaskAction action)
        {
            Add($"Successfully executed {action.Name}");
        }
    }
}
