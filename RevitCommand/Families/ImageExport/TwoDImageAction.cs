using System;

namespace RevitCommand.Families.ImageExport
{
    public class TwoDImageAction : ATaskActionCommand<TwoDImageAction, TwoDImageRevitCommand>
    {
        public TwoDImageAction() : base("2D Image Export", new Guid("52f427dd-6db4-43bd-b4d2-c7327ae274fa")) { }
    }
}
