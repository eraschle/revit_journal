using DataSource.Model.FileSystem;
using RevitCommand.Families.SharedParameter;
using RevitJournal.Journal.Command;
using RevitJournal.Journal.Command.Document;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class MergeAllParametersCommand : AJournalExternalCommand<MergeParametersCommandData>
    {
        public MergeAllParametersCommand()
            : base(new MergeParametersCommandData(), "Merge Shared [ALL]")
        {
            Parameters.Add(new SharedFileCommandParameter(MergeParametersCommandData.KeySharedFile));
        }

        public override void PreExecutionTask(RevitFamily family)
        {
            AddParameterToJournal(MergeParametersCommandData.KeySharedFile);
        }

        public override bool DependsOnCommand(IJournalCommand command)
        {
            return command is DocumentSaveCommand;
        }
    }
}
