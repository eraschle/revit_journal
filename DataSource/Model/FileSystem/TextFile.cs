namespace DataSource.Model.FileSystem
{
    public class TextFile : AFileNode
    {
        public const string TextExtension = "txt";

        public override string FileExtension { get; } = TextExtension;
    }
}
