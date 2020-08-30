using DataSource.Model.FileSystem;

namespace RevitJournal.Journal.Execution
{
    public class RecordeJournalFile : AFile
    {
        public const string TaskJournalExtension = "txt";

        public string JournalDirectory { get { return ParentFolder; } }

        public override string ToString()
        {
            return Name;
        }

        protected override string GetExtension()
        {
            return TaskJournalExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(TaskJournalFile);
        }
    }
}
