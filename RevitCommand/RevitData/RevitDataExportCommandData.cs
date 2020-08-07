using RevitCommand.Families;
using System;

namespace RevitCommand.RevitData
{
    public class RevitDataExportCommandData : ARevitExternalCommandData
    {
        public static readonly string ProductDataDir = "ProductDataDir";

        public override Guid AddinId { get; } = new Guid("6d6b38cf-cb70-4a73-9e2d-56fc23cd9cbf");

        public override string CommandName { get; } = "Update metadata";

        protected override Type CommandDataType { get { return GetType(); } }

        protected override string ExternalCommandName { get { return nameof(RevitDataExportExternalCommand); } }
    }
}
