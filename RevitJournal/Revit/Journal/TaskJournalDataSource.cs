using DataSource.DataSource;
using RevitAction.Action;
using RevitJournal.Revit.Command;
using RevitJournal.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.System;

namespace RevitJournal.Revit.Journal
{
    public class TaskJournalDataSource : AFileDataSource<RevitTask, TaskJournalFile>
    {
        private static readonly RevitStartCommand startRevit = new RevitStartCommand();
        private static readonly RevitCloseCommand closeRevit = new RevitCloseCommand();

        public override RevitTask Read()
        {
            throw new NotImplementedException();
        }

        public override void Write(RevitTask model)
        {
            if (model is null) { throw new ArgumentNullException(nameof(model)); }

            var content = Build(model.Actions);
            File.WriteAllText(FileNode.FullPath, content, Encoding.Default);
        }

        public override void SetFile(TaskJournalFile fileNode)
        {
            if (fileNode is null) { throw new ArgumentNullException(nameof(fileNode)); }

            var formats = new string[] { DateUtils.Hour, DateUtils.Minute, DateUtils.Seconds, DateUtils.Milliseconds };
            fileNode.AddSuffixes(DateUtils.AsString(Constant.Minus, formats));
            base.SetFile(fileNode);
        }

        internal string Build(IEnumerable<ITaskAction> actions)
        {
            var journalLines = new StringBuilder();
            AddLines(ref journalLines, startRevit.Commands);
            foreach (var action in actions)
            {
                if (action is ITaskActionJournal journal)
                {
                    AddJournal(ref journalLines, journal);
                }
                else if (action is ITaskActionCommand command)
                {
                    AddCommand(ref journalLines, command);
                }
            }
            AddLines(ref journalLines, closeRevit.Commands);
            return journalLines.ToString();
        }

        private void AddCommand(ref StringBuilder commands, ITaskActionCommand action)
        {
            var commandLines = BuildCommand(action);
            AddLines(ref commands, commandLines);
        }

        private void AddJournal(ref StringBuilder commands, ITaskActionJournal action)
        {
            AddLines(ref commands, action.Commands);
        }

        private void AddLines(ref StringBuilder commands, IEnumerable<string> commandLines)
        {
            foreach (var command in commandLines)
            {
                commands.AppendLine(command);
            }
            commands.AppendLine(Environment.NewLine);
        }

        private IEnumerable<string> BuildCommand(ITaskActionCommand command)
        {
            var parameters = command.Parameters.Where(par => par.IsJournalParameter);
            var journalData = new StringBuilder();
            journalData.Append($"Jrn.Data \"APIStringStringMapJournalData\", {parameters.Count()}");
            foreach (var parameter in parameters)
            {
                journalData.Append($", \"{parameter.JournalKey}\", \"{parameter.GetJournalValue()}\"");
            }

            return new string[]
            {
                $"Jrn.RibbonEvent \"Execute external command:{command.ActionId}:{command.TaskInfo.TaskNamespace}\"",
                journalData.ToString()
            };
        }
    }
}
