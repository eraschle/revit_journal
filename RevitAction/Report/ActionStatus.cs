using RevitAction.Report.Message;
using System;

namespace RevitAction.Report
{
    public enum ActionStatus
    {
        Unknown, Started, Finished, Error
    }

    public static class ActionStatusUtil
    {
        public static ActionStatus ToEnum(ReportMessage report)
        {
            if(report is null || Enum.TryParse<ActionStatus>(report.Message, out var status) == false)
            {
                status = ActionStatus.Unknown;
            }
            return status;
        }
    }

}
