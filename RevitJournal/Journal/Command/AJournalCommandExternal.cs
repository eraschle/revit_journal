using RevitCommand.Families;
using System.Collections.Generic;

namespace RevitJournal.Journal.Command
{
    public abstract class AJournalExternalCommand<TCommandData>
        : AJournalCommand, IJournalCommandExternal
          where TCommandData : IRevitExternalCommandData
    {
        protected TCommandData ExternalData;
        protected AJournalExternalCommand(TCommandData commandData, string commandName)
            : base(commandName, false)
        {
            ExternalData = commandData;
        }

        public override IEnumerable<string> Commands
        {
            get
            {
                return new string[]
                {
                    JournalCommandBuilder.BuildExternalAddinLaunch(ExternalData.AddinId, ExternalData.Namespace),
                    JournalCommandBuilder.BuildExternalAddinCommandData(JournalData)
                };
            }
        }

        public IDictionary<string, string> JournalData { get; } = new Dictionary<string, string>();

        public IRevitExternalCommandData CommandData { get { return ExternalData; } }

        protected void AddParameterToJournal(string key)
        {
            if (HasParameterByJounralKey(key, out var parameter))
            {
                AddParameterToJournal(key, parameter.Value);
            }
        }

        protected void AddParameterToJournal(string key, string value)
        {
            if (JournalData.ContainsKey(key)) { return; }

            JournalData.Add(key, value);
        }
    }
}
