using DataSource.Model.FileSystem;
using RevitJournal.Journal.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Journal.Execution
{
    public class JournalProcessFile : AFile
    {
        public const string JournalProcessExtension = "txt";
        public string JournalDirectory { get { return ParentFolder; } }

        public IList<JournalProcessCommand> CommandLines { get; internal set; }

        public int LastCommandLine
        {
            get
            {
                var lastLineNumber = 0;
                for (int idx = 0; idx < CommandLines.Count; idx++)
                {
                    lastLineNumber = idx + 1;

                    if (IsCommandLine(CommandLines[idx]) == false) { continue; }

                }
                return lastLineNumber;
            }
        }

        public JournalProcessCommand ByLineNumber(int lineNumber)
        {
            return CommandLines.Where(cmd => cmd.LineNumber == lineNumber).FirstOrDefault();
        }

        private static bool IsCommandLine(JournalProcessCommand line)
        {
            return line.Command.StartsWith(JournalCommandBuilder.JournalCommand,
                                           StringComparison.CurrentCulture);
        }

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
