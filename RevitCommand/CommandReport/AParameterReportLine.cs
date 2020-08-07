namespace RevitCommand.Reports
{
    public abstract class AParameterReportLine : MessageReportLine
    {
        protected AParameterReportLine(string action) : base(action) { }

        public string ParameterName { get; set; }

        public bool IsInstance { get; set; }

        public string ParameterGroup { get; set; }
    }
}
