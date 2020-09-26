using DataSource.Helper;
using DataSource.Model.FileSystem;
using RevitJournal.Tasks;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace RevitJournal.Revit.Journal
{
    internal static class TaskJournalCreator
    {
        private const string SuffixFormatString = "HHmmssfff";

        internal static TaskJournalFile Create(RevitTask journalTask, DirectoryNode directory)
        {
            var journalFile = journalTask.SourceFile.ChangeDirectory<TaskJournalFile>(directory);
            var suffix = DateTime.Now.ToString(SuffixFormatString, CultureInfo.CurrentCulture);
            journalFile.AddSuffixes(suffix);
            var content = JournalBuilder.Build(journalTask.Actions);
            File.WriteAllText(journalFile.FullPath, content, Encoding.Default);
            return journalFile;
        }
    }
}
