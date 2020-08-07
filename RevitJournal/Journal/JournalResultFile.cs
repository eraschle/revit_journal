using DataSource.Model.FileSystem;
using RevitJournal.Journal.Execution;

namespace RevitJournal.Journal.Model
{
    public class JournalResultFile : AFile
    {
        public const string JournalResultExtension = "json";

        protected override string GetExtension()
        {
            return JournalResultExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(JournalRevitFile);
        }
    }
}
