using DataSource.Model.FileSystem;
using RevitCommand.Families.ImageExport;
using RevitJournal.Journal.Command;

namespace RevitJournal.Revit.Commands.ImageExport
{
    public class ThreeDImageExportExternalJounralCommand : AJournalExternalCommand<ThreeDImageExportCommandData>
    {
        public const string DefaultParameterName = "Background Image";

        public ThreeDImageExportExternalJounralCommand()
            : base(new ThreeDImageExportCommandData(), "3D Image Export")
        {
            Parameters.Add(new CommandParameterExternal(
                ThreeDImageExportCommandData.KeyBackground,
                DefaultParameterName,
                JournalParameterType.ImageFile, true));
        }

        public override void PreExecutionTask(RevitFamily family)
        {
            AddParameterToJournal(ThreeDImageExportCommandData.KeyBackground);
        }
    }
}
