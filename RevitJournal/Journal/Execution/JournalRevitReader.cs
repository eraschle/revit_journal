using DataSource.Helper;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RevitJournal.Journal.Execution
{
    internal static class JournalRevitReader
    {
        internal const string Quotes = "\"";

        internal const string JournalStart = "Set Jrn = CrsJournalScript";
        internal const string JournalFilePath = "started journal file playback of";
        internal const string JournalExit = "Journal Exit";
        internal static readonly Regex JournalCommandStop = new Regex(@"stopped at line \d+ journal file playback");
        internal static readonly Regex JournalCommandExecuted = new Regex(@"at line number \d+");
        internal static readonly Regex JournalLineNumber = new Regex(@"\d+");

        internal static string Read(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fileStream, Encoding.Default))
            {
                return reader.ReadToEnd();
            }
        }

        internal static JournalRevitFile Create(string filePath, string content)
        {
            var revitJournal = new JournalRevitFile { FullPath = filePath };
            var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) { continue; }

                if (IsJournalCommand(line, out var journalCommand))
                {
                    revitJournal.Commands.Add(journalCommand);
                }
                else if (IsRevitCommand(line, out var revitCommand))
                {
                    revitJournal.Commands.Add(revitCommand);
                }
                else if (IsTaskJournal(line, out var processJournal))
                {
                    revitJournal.JournalProcessPath = processJournal;
                }
            }
            return revitJournal;
        }

        private static bool IsTaskJournal(string line, out string processJournalPath)
        {
            processJournalPath = GetJournalFilePath(line);
            return ExstingFile(processJournalPath);
        }


        private static bool ExstingFile(string filePath)
        {
            return string.IsNullOrEmpty(filePath) == false && File.Exists(filePath);
        }

        private static string GetJournalFilePath(string line)
        {
            var journalFilePath = string.Empty;
            if (line.Contains(JournalFilePath))
            {
                journalFilePath = line.Trim().Split(Constant.SpaceChar).Last();
                if (journalFilePath != null)
                {
                    journalFilePath = journalFilePath.Replace(Quotes, string.Empty);
                }
            }
            return journalFilePath;
        }

        public static bool IsRevitCommand(string line, out JournalRevitCommand command)
        {
            command = null;
            var isStart = IsJournalStart(line);
            var isEnd = IsJournalExit(line);
            if (isStart || isEnd)
            {
                command = new JournalRevitCommand(line)
                {
                    IsJournalStart = isStart,
                    IsJournalExit = isEnd
                };
            }
            return command != null;
        }

        public static bool IsJournalCommand(string line, out JournalRevitCommand command)
        {
            return IsCommandStop(line, out command) || IsCommandExecuted(line, out command);
        }

        public static bool IsJournalStart(string line)
        {
            return line.Contains(JournalStart);
        }

        public static bool IsJournalExit(string line)
        {
            return line.Contains(JournalExit);
        }

        public static bool IsCommandExecuted(string line, out JournalRevitCommand command)
        {
            command = null;
            var match = JournalCommandExecuted.Match(line);
            if (match.Success)
            {
                var lineNumber = GetCommandLineIndex(match);
                command = new JournalRevitCommand(lineNumber, line);
            }
            return command != null;
        }

        public static bool IsCommandStop(string line, out JournalRevitCommand command)
        {
            command = null;
            var match = JournalCommandStop.Match(line);
            if (match.Success)
            {
                var lineNumber = GetCommandLineIndex(match);
                command = new JournalRevitCommand(lineNumber, line)
                {
                    IsJournalCommandStop = true
                };
            }
            return command != null;
        }

        private static int GetCommandLineIndex(Match match)
        {
            var lineNumber = -1;
            var numberRegex = JournalLineNumber;
            var result = numberRegex.Match(match.Value);
            if (int.TryParse(result.Value, out var value))
            {
                lineNumber = value;
            }
            return lineNumber;
        }

    }
}
