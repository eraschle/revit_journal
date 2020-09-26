namespace DataSource.Model.FileSystem
{
    public class LogFile : AFileNode
    {
        public const string LogExtension = "log";

        public override string FileExtension { get; } = LogExtension;
    }
}
