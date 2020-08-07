using DataSource.Model.FileSystem;
using Newtonsoft.Json;
using RevitCommand.CommandReport;
using System.Collections.Generic;

namespace RevitCommand.Reports
{
    public class Report
    {
        private const string ErrorSuffix = "ERROR";
        private const string ReportSuffix = "report";

        private readonly ReportJsonDataSource DataSource;

        public Report(RevitFile revitFile)
        {
            DataSource = new ReportJsonDataSource(revitFile);
        }

        public List<MessageReportLine> ReportLines { set; get; }
            = new List<MessageReportLine>();

        public void AddLine(MessageReportLine reportLine)
        {
            if (reportLine is null || ReportLines.Contains(reportLine)) { return; }

            ReportLines.Add(reportLine);
        }

        public void AddLines(IEnumerable<MessageReportLine> reportLines)
        {
            if (reportLines is null) { return; }

            foreach (var line in reportLines)
            {
                AddLine(line);
            }
        }

        [JsonIgnore]
        public bool IsEmpty { get { return ReportLines.Count == 0; } }

        public void Write(params string[] suffixes)
        {
            if (IsEmpty) { return; }

            if (suffixes is null || suffixes.Length == 0)
            {
                suffixes = new string[1] { ReportSuffix };
            }
            if (ReportContainsError())
            {
                var tmpSuffixes = new List<string>(suffixes) { ErrorSuffix };
                suffixes = tmpSuffixes.ToArray();
            }
            DataSource.AddFileNameSuffix(suffixes);
            DataSource.Write(this);
        }

        private bool ReportContainsError()
        {
            var hasError = false;
            foreach (var reportLine in ReportLines)
            {
                hasError |= reportLine.IsError;
                if (hasError == true) { break; }
            }
            return hasError;
        }
    }
}
