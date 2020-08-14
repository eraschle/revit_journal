using RevitCommand;
using RevitCommand.Families.ImageExport;
using System;

namespace RevitJournal.Revit.Commands.ImageExport
{
    public class TwoDImageAction : ATaskActionCommand
    {
        public TwoDImageAction() : base("2D Image Export") { }

        public override Guid AddinId
        {
            get { return new Guid("52f427dd-6db4-43bd-b4d2-c7327ae274fa"); }
        }

        public override string Namespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(TwoDImageRevitCommand); }
        }
    }
}
