using RevitCommand.Families.ImageExport;
using RevitJournal.Journal.Command;

namespace RevitJournal.Revit.Commands.ImageExport
{
    public class TwoDImageExportExternalJounralCommand : AJournalExternalCommand<TwoDImageExportCommandData>
    {
        public TwoDImageExportExternalJounralCommand()
            : base(new TwoDImageExportCommandData(), "2D Image Export") { }

    }
}
