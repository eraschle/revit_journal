using System;

namespace RevitCommand.Families.Metadata
{
    public class CreateMetadataCommandData : ARevitExternalCommandData
    {
        public const string KeyLibrary = "Library";

        public override Guid AddinId { get; } = new Guid("7d3a1639-4384-4488-b34c-08f29aebac2f");

        public override string CommandName { get; } = "Create family metadata";

        protected override Type CommandDataType { get { return GetType(); } }

        protected override string ExternalCommandName { get { return nameof(CreateMetadataExternalCommand); } }
    }
}
