namespace RevitCommand.Reports
{
    public abstract class AReportLine
    {
        protected AReportLine(string action)
        {
            Action = action;
        }

        public string Action { get; private set; }
    }
}
