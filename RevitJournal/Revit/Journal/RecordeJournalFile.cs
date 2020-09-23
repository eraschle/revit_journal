using DataSource.Model.FileSystem;

namespace RevitJournal.Revit.Journal
{
    public class RecordeJournalFile : TextFile { }


    public class RecordeJournalNullFile : RecordeJournalFile
    {
        public RecordeJournalNullFile()
        {
            Name = "No Record Journal file";
        }
    }
}
