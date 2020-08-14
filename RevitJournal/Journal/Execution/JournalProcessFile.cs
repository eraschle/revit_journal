using DataSource.Model.FileSystem;

namespace RevitJournal.Journal.Execution
{
    public class JournalProcessFile : AFile
    {
        public const string JournalProcessExtension = "txt";
        public string JournalDirectory { get { return ParentFolder; } }

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
            return nameof(JournalProcessFile);
        }
    }
}
