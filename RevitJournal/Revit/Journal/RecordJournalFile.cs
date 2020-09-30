using DataSource.Model.FileSystem;

namespace RevitJournal.Revit.Journal
{
    public class RecordJournalFile : TextFile { }


    public class RecordJournalNullFile : RecordJournalFile
    {
        public RecordJournalNullFile()
        {
            Name = "No Record Journal file";
        }
    }
}
