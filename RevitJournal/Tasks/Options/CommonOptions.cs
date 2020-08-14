namespace RevitJournal.Tasks.Options
{
    public class CommonOptions
    {
        public string RevitDirectory { get; set; }

        public string JournalDirectory { get; set; }

        public bool DeleteRevitBackup { get; set; } = true;
    }
}
