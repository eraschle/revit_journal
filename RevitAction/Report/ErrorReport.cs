using System;
using System.Text;

namespace RevitAction.Report
{
    public class ErrorReport : AReport
    {
        public void AddNull(string propertyName)
        {
            Add($"{propertyName} is NULL");
        }

        public void Add(Exception exception, bool stackTrace = false)
        {
            var message = new StringBuilder();
            message.AppendLine(exception.Message);
            if (stackTrace)
            {
                message.AppendLine(exception.StackTrace);
            }
            Add(message.ToString());
        }
    }
}
