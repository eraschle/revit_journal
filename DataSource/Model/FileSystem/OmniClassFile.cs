namespace DataSource.Model.FileSystem
{
    public class OmniClassFile : AFile
    {
        public const string FileExtension = "txt";
        public const string FileName = "OmniClassTaxonomy";

        protected override string GetExtension()
        {
            return FileExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(OmniClassFile);
        }
    }
}
