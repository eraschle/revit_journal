using DataSource.Model.FileSystem;
using System;

namespace RevitJournal.Revit.Journal
{
    public class TaskJournalFile : TextFile
    {
        private string fileName;

        public void SetFileName(RevitFamilyFile file)
        {
            if(file is null) { throw new ArgumentNullException(nameof(file)); }

            fileName = file.NameWithoutExtension;
        }

        public string GetFileName()
        {
            return fileName;
        }
    }

    public class TaskJournalNullFile : TaskJournalFile
    {
        public TaskJournalNullFile()
        {
            Name = "No Task Journal file";
        }
    }
}
