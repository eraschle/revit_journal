using DataSource.Model.FileSystem;
using RevitJournal.Journal.Command;
using RevitJournal.Journal.Command.Document;
using RevitJournal.Journal.Execution;
using System;
using System.Collections.Generic;

namespace RevitJournal.Journal
{
    public class JournalTask
    {
        public RevitFamily Family { get; private set; }

        public RevitFamilyFile RevitFile { get { return Family.RevitFile; } }

        public JournalOption TaskOption { get; private set; }

        public JournalProcessFile JournalProcess { get; private set; }

        public IList<IJournalCommand> Commands { get; private set; }

        public JournalTask(RevitFamily family, JournalOption taskOption)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }
            if (taskOption is null) { throw new ArgumentNullException(nameof(taskOption)); }

            Family = family;
            TaskOption = taskOption;
            Commands = new List<IJournalCommand>();
        }

        public string Name { get { return Family.RevitFile.Name; } }

        public void ClearCommands()
        {
            Commands.Clear();
        }

        public void AddCommand(IJournalCommand command)
        {
            if (command.IsDefault == true) { return; }
            if (Commands.Contains(command)) { return; }

            Commands.Add(command);
        }

        public bool HasExternalCommands(out ICollection<IJournalCommandExternal> externalCommands)
        {
            externalCommands = new List<IJournalCommandExternal>();
            foreach (var command in Commands)
            {
                if (!(command is IJournalCommandExternal externalCommand)) { continue; }

                externalCommands.Add(externalCommand);
            }
            return externalCommands.Count > 0;
        }

        public void CreateJournalProcess(string journalDirectory)
        {
            JournalProcess = JournalProcessCreator.Create(this, journalDirectory);
        }

        public void DeleteJournalProcess()
        {
            if (JournalProcess is null) { return; }

            JournalProcess.Delete();
        }

        internal void PreExecution(JournalOption option)
        {
            if (option.BackupRevitFile)
            {
                var revitFile = Family.RevitFile;
                revitFile.CopyTo<RevitFamilyFile>(option.GetBackupFile(revitFile), true);
            }

            foreach (var command in Commands)
            {
                command.PreExecutionTask(Family);
            }
        }

        internal JournalResult PostExecution(JournalResult result)
        {
            foreach (var command in Commands)
            {
                command.PostExecutionTask(result);
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is JournalTask task &&
                   EqualityComparer<RevitFamilyFile>.Default.Equals(Family.RevitFile, task.Family.RevitFile);
        }

        public override int GetHashCode()
        {
            return 1472110217 + EqualityComparer<RevitFamilyFile>.Default.GetHashCode(Family.RevitFile);
        }
    }
}
