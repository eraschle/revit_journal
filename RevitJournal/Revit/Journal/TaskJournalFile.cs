using DataSource.Model.FileSystem;

namespace RevitJournal.Revit.Journal
{
    public class TaskJournalFile : TextFile { }


    public class TaskJournalNullFile : TaskJournalFile
    {
        public TaskJournalNullFile()
        {
            Name = "No Task Journal file";
        }
    }
}
