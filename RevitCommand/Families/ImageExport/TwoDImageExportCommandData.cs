using System;

namespace RevitCommand.Families.ImageExport
{
    public class TwoDImageExportCommandData : ARevitExternalCommandData
    {
        public override Guid AddinId { get; } = new Guid("52f427dd-6db4-43bd-b4d2-c7327ae274fa");

        public override string CommandName { get; } = "Symbol Image Export";

        protected override Type CommandDataType { get { return GetType(); } }

        protected override string ExternalCommandName { get { return nameof(TwoDImageExportExternalCommand); } }
    }
}
