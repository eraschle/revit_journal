using Newtonsoft.Json;

namespace RevitCommand.Reports
{
    public class MessageReportLine : AReportLine
    {
        public MessageReportLine(string action) : base(action) { }

        public string Meassage { get; set; } = null;

        public string ErrorMessage { get; set; } = null;

        [JsonIgnore]
        public bool IsError { get { return string.IsNullOrEmpty(ErrorMessage) == false; } }
    }
}
