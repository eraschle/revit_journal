using System;

namespace RevitCommand.Families.Metadata
{
    public class EditMetadataCommandData : ARevitExternalCommandData
    {
        public static readonly string KeyLibrary = "Library";

        public override Guid AddinId { get; } = new Guid("318cfd92-27ee-47a8-bb0f-840a9ff0b081");

        public override string CommandName { get; } = "Edit metadata";

        protected override Type CommandDataType { get { return GetType(); } }

        protected override string ExternalCommandName { get { return nameof(EditMetadataExternalCommand); } }
    }
}
