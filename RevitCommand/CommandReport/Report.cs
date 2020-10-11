using DataSource.DataSource.Json;
using DataSource.Models.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RevitCommand.Reports
{
    public class Report
    {
        private const string ErrorSuffix = "ERROR";
        private const string ReportSuffix = "report";

        private readonly JsonDataSource<Report> dataSource;

        public Report(RevitFamilyFile revitFile)
        {
            if (revitFile is null) { throw new ArgumentNullException(nameof(revitFile)); }

            dataSource = new JsonDataSource<Report>();
            var jsonFile = revitFile.ChangeExtension<JsonFile>();
            dataSource.SetFile(jsonFile);
        }

        public List<MessageReportLine> ReportLines { get; } = new List<MessageReportLine>();

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

        public void Write()
        {
            if (IsEmpty) { return; }

            var suffixes = new List<string> { ReportSuffix };
            if (ReportContainsError())
            {
                suffixes.Add(ErrorSuffix);
            }
            dataSource.FileNode.AddSuffixes(suffixes.ToArray());
            dataSource.Write(this);
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
