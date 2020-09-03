using DataSource.Model.FileSystem;

namespace RevitAction.Report
{
    public interface ITaskReport
    {
        RevitFamilyFile SourceFile { get; }

        RevitFamilyFile ResultFile { get; set; }

        RevitFamilyFile BackupFile { get; set; }
    }
}
