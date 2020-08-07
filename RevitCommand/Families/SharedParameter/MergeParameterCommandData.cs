using System;
using System.Collections.Generic;

namespace RevitCommand.Families.SharedParameter
{
    public class MergeParameterCommandData : ARevitExternalCommandData
    {
        public const string KeySharedFile = "SharedFile";
        public const string KeySharedParameters = "SharedParameters";
        public const string KeyAddifNot = "AddIfNot";
        public const string KeyAddifNotIsInstance = "AddIfNotInstance";
        public const string KeyAddifNotParameterGroup = "AddIfNotParameterGroup";

        public override Guid AddinId { get; } = new Guid("2c0ddb9f-ec48-4c34-bc96-1105eb3a1637");

        public override string CommandName { get; } = "Manage Shared Parameter";

        protected override Type CommandDataType { get { return GetType(); } }

        protected override string ExternalCommandName { get { return nameof(MergeParameterExternalCommand); } }

        public override HashSet<string> JournalDataKeys { get; } = new HashSet<string> { KeySharedFile, KeyAddifNot };
    }
}
