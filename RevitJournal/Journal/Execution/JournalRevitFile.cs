using DataSource.Model.FileSystem;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RevitJournal.Journal.Execution
{
    public class JournalRevitFile : AFile
    {
        public const string JournalRevitExtension = "txt";

        public string JournalProcessPath { get; internal set; }

        public bool HasJournalProcessPath
        {
            get
            {
                return string.IsNullOrEmpty(JournalProcessPath) == false &&
                  File.Exists(JournalProcessPath);
            }
        }

        public IList<JournalRevitCommand> Commands { get; internal set; }
            = new List<JournalRevitCommand>();

        internal int LastCommandLineNumber { get { return LastCommandLine(Commands); } }

        private static int LastCommandLine(IEnumerable<JournalRevitCommand> commands)
        {
            if (commands.Any() == false) { return -1; }

            var last = commands.LastOrDefault();
            if (last is null) { return -1; }

            return last.LineNumber;
        }

        public bool HasError(JournalProcessFile journalProcess)
        {
            return HasBeenStarted() && HasStoppedBeforeLast(journalProcess);
        }

        public bool AllCommandExecuted(JournalProcessFile process)
        {
            return process != null && process.LastCommandLine == LastCommandLineNumber;
        }

        public bool HasStoppedBeforeLast(JournalProcessFile process)
        {
            var commands = Commands.Where(cmd => cmd.IsJournalCommandStop);
            var lastCommandLine = LastCommandLine(commands);
            return process != null && lastCommandLine != -1 && process.LastCommandLine != lastCommandLine;
        }

        public bool HasBeenStarted()
        {
            return Commands.Any(cmd => cmd.IsJournalStart);
        }

        public bool HasBeenFinished()
        {
            return Commands.Any(cmd => cmd.IsJournalExit);
        }

        protected override string GetExtension()
        {
            return JournalRevitExtension;
        }

        protected override string GetTypeName()
        {
            return nameof(JournalRevitFile);
        }
    }
}
