namespace DataSource.Model.FileSystem
{
    public class JsonFile : AFileNode
    {
        public const string JsonExtension = "json";

        public override string FileExtension { get; } = JsonExtension;
    }
}
