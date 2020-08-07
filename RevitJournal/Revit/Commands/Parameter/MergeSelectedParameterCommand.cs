using DataSource;
using DataSource.Model.FileSystem;
using RevitCommand.Families.SharedParameter;
using RevitJournal.Journal.Command;
using RevitJournal.Journal.Command.Document;
using System.Linq;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class MergeSelectedParameterCommand : AJournalExternalCommand<MergeParameterCommandData>
    {
        public MergeSelectedParameterCommand() : base(new MergeParameterCommandData(), "Merge Shared [Selectable]")
        {
            var fileCommand = new SharedFileCommandParameter(MergeParameterCommandData.KeySharedFile);
            var parameterCommand = new SharedParameterCommandParameter(MergeParameterCommandData.KeySharedParameters,
                                                                       fileCommand.GetSharedParameters);
            var parameterGroups = ProductDataManager.Get().ParameterGroups().Select(grp => grp.Name).ToList();

            Parameters.Add(fileCommand);
            Parameters.Add(parameterCommand);
            Parameters.Add(new AddIfNotCommandParameter(MergeParameterCommandData.KeyAddifNot));
            Parameters.Add(new CommandParameterExternal(MergeParameterCommandData.KeyAddifNotIsInstance, "Is Instance", JournalParameterType.Boolean, true));
            Parameters.Add(new CommandParameterExternalSelect(MergeParameterCommandData.KeyAddifNotParameterGroup, "Parameter Group", true, parameterGroups));
        }

        public override void PreExecutionTask(RevitFamily family)
        {
            AddParameterToJournal(MergeParameterCommandData.KeySharedFile);
            AddParameterToJournal(MergeParameterCommandData.KeySharedParameters);
            AddParameterToJournal(MergeParameterCommandData.KeyAddifNot);
            AddParameterToJournal(MergeParameterCommandData.KeyAddifNotIsInstance);
            AddParameterToJournal(MergeParameterCommandData.KeyAddifNotParameterGroup);
        }

        public override bool DependsOnCommand(IJournalCommand command)
        {
            return command is DocumentSaveCommand;
        }
    }
}
