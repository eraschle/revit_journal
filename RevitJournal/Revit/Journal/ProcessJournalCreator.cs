using DataSource.Helper;
using DataSource.Model.FileSystem;
using RevitJournal.Revit.Journal;
using System;
using System.IO;
using System.Text;

namespace RevitJournal.Journal.Execution
{
    internal static class ProcessJournalCreator
    {
        private const string SuffixFormatString = "HHmmssfff";

        internal static ProcessJournalFile Create(RevitTask journalTask, string journalDirectory)
        {
            var journalFile = GetJournalFile(journalTask.Family, journalDirectory);
            var content = JournalBuilder.Build(journalTask);
            File.WriteAllText(journalFile.FullPath, content, Encoding.Default);
            return journalFile;
        }
     

        private static ProcessJournalFile GetJournalFile(RevitFamily family, string journalDirectory)
        {
            var journalName = GetJournalFileName(family.RevitFile);
            var journalFile = family.RevitFile
                .ChangeFileName<RevitFamilyFile>(journalName)
                .ChangeDirectory<RevitFamilyFile>(journalDirectory)
                .ChangeExtension<ProcessJournalFile>(ProcessJournalFile.JournalProcessExtension);
            return journalFile;
        }

        private static string GetJournalFileName(RevitFamilyFile revitFile)
        {
            var suffix = DateTime.Now.ToString(SuffixFormatString);
            var fileName = string.Concat(revitFile.Name, Constant.Underline, suffix);
            return fileName.Replace(Constant.Space, Constant.Underline);
        }
    }
}
