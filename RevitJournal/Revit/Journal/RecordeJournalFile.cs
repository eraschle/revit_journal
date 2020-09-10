using DataSource.Model.FileSystem;

namespace RevitJournal.Revit.Journal
{
    public class RecordeJournalFile : AFile
    {
        public const string TaskJournalExtension = "txt";

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
