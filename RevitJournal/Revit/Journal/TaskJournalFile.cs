using DataSource.Model.FileSystem;

namespace RevitJournal.Revit.Journal
{
    public class TaskJournalFile : AFile
    {
        public const string JournalProcessExtension = "txt";

        public override string ToString()
        {
            return Name;
        }

        protected override string GetExtension()
        {
            return JournalProcessExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(TaskJournalFile);
        }
    }
}
