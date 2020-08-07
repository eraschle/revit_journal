using DataSource.Model.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Journal.Command
{
    public abstract class AJournalCommand : IJournalCommand
    {
        protected AJournalCommand(string commandName, bool defaultCommand = false)
        {
            Name = commandName;
            IsDefault = defaultCommand;
        }

        public string Name { get; private set; }

        public bool HasParameters { get { return Parameters != null && Parameters.Count > 0; } }

        public IList<ICommandParameter> Parameters { get; } 
            = new List<ICommandParameter>();

        protected bool HasParameterByJounralKey(string journalKey, out ICommandParameter parameter)
        {
            parameter = Parameters.FirstOrDefault(par => IsJournalKey(par, journalKey));
            return parameter != null;
        }

        private bool IsJournalKey(ICommandParameter parameter, string journalKey)
        {
            return parameter.JournalKey.Equals(journalKey, StringComparison.CurrentCulture);
        }

        public bool IsDefault { get; private set; }
        
        public abstract IEnumerable<string> Commands { get; }

        public virtual void PreExecutionTask(RevitFamily family) { }
        
        public virtual void PostExecutionTask(JournalResult result) { }

        public virtual bool DependsOnCommand(IJournalCommand command)
        {
            return false;
        }
    }
}
