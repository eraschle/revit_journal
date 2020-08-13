using DataSource.DataSource.Json;
using DataSource.Model.FileSystem;
using RevitCommand.Reports;

namespace RevitCommand.CommandReport
{
    public class ReportJsonDataSource
    {
        public JsonDataSource<Report> JsonDataSource { get; private set; }

        public ReportJsonDataSource(RevitFamilyFile revitFile)
        {
            JsonDataSource = new JsonDataSource<Report>(revitFile);
        }

        public void AddFileNameSuffix(params string[] suffixes)
        {
            JsonDataSource.AddFileNameSuffix(suffixes);
        }

        public bool DataSourceExist
        {
            get { return JsonDataSource.JsonFile.Exist; }
        }

        public Report Read()
        {
            return JsonDataSource.Read();
        }

        public void Write(Report model)
        {
            JsonDataSource.Write(model);
        }
    }
}