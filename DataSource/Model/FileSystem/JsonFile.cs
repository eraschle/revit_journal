namespace DataSource.Model.FileSystem
{
    public class JsonFile : AFile
    {
        public const string FileExtension = "json";

        protected override string GetExtension()
        {
            return FileExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(JsonFile);
        }
    }
}
