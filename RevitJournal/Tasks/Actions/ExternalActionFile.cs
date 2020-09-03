using DataSource.Model.FileSystem;

namespace RevitJournal.Tasks.Actions
{
    public class ExternalActionFile : JsonFile
    {
        public const string FileExtension = "action";

        protected override string GetExtension()
        {
            return FileExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(ExternalActionFile);
        }
    }
}
