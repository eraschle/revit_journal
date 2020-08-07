using DataSource.Model.FileSystem;
using RevitCommand.Families.Metadata;
using RevitJournal.Journal.Command;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class MetadataEditJournalCommand : AJournalExternalCommand<EditMetadataCommandData>
    {
        public MetadataEditJournalCommand() 
            : base(new EditMetadataCommandData(), "Change Metadata") { }

        public override void PreExecutionTask(RevitFamily family)
        {
            AddParameterToJournal(EditMetadataCommandData.KeyLibrary, family.LibraryPath);
        }
    }
}
