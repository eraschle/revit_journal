using DataSource.Helper;
using DataSource.Model.FileSystem;
using RevitJournal.Journal.Command;
using System;
using System.IO;
using System.Text;

namespace RevitJournal.Journal.Execution
{
    internal static class JournalProcessCreator
    {
        private const string SuffixFormatString = "HHmmssfff";

        internal static JournalProcessFile Create(RevitTask journalTask, string journalDirectory)
        {
            var journalFile = GetJournalFile(journalTask.Family, journalDirectory);
            var content = JournalBuilder.Build(journalTask);
            File.WriteAllText(journalFile.FullPath, content, Encoding.Default);
            return journalFile;
        }
     

        private static JournalProcessFile GetJournalFile(RevitFamily family, string journalDirectory)
        {
            var journalName = GetJournalFileName(family.RevitFile);
            var journalFile = family.RevitFile
                .ChangeFileName<RevitFamilyFile>(journalName)
                .ChangeDirectory<RevitFamilyFile>(journalDirectory)
                .ChangeExtension<JournalProcessFile>(JournalProcessFile.JournalProcessExtension);
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
