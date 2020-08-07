using DataSource.Model.FileSystem;
using RevitCommand.Families.Metadata;
using RevitJournal.Journal.Command;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class MetadataJournalCommand : AJournalExternalCommand<CreateMetadataCommandData>
    {
        public MetadataJournalCommand() 
            : base(new CreateMetadataCommandData(), "Create Metadata") { }

        public override void PreExecutionTask(RevitFamily family)
        {
            family.WriteMetaData();
            AddParameterToJournal(CreateMetadataCommandData.KeyLibrary, family.LibraryPath);
        }
    }
}
