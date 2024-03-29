﻿using DataSource.Helper;
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

        internal static JournalProcessFile Create(JournalTask journalTask, string journalDirectory)
        {
            var journalFile = GetJournalFile(journalTask.Family, journalDirectory);
            var content = JournalBuilder.Build(journalTask, out var journalCommands);
            File.WriteAllText(journalFile.FullPath, content, Encoding.Default);
            journalFile.CommandLines = journalCommands;
            return journalFile;
        }

     

        private static JournalProcessFile GetJournalFile(RevitFamily family, string journalDirectory)
        {
            var journalName = GetJournalFileName(family.RevitFile);
            var journalFile = family.RevitFile
                .ChangeFileName<RevitFile>(journalName)
                .ChangeDirectory<RevitFile>(journalDirectory)
                .ChangeExtension<JournalProcessFile>(JournalProcessFile.JournalProcessExtension);
            return journalFile;
        }

        private static string GetJournalFileName(RevitFile revitFile)
        {
            var suffix = DateTime.Now.ToString(SuffixFormatString);
            var fileName = string.Concat(revitFile.Name, Constant.Underline, suffix);
            return fileName.Replace(Constant.Space, Constant.Underline);
        }
    }
}
