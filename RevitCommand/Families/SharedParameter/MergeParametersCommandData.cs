using System;
using System.Collections.Generic;

namespace RevitCommand.Families.SharedParameter
{
    public class MergeParametersCommandData : ARevitExternalCommandData
    {
        public const string KeySharedFile = "SharedFile";

        public override Guid AddinId { get; } = new Guid("af072261-088e-42d3-bf5e-39fc99ea5736");

        public override string CommandName { get; } = "Merge All Shared Parameters";

        protected override Type CommandDataType { get { return GetType(); } }

        protected override string ExternalCommandName { get { return nameof(MergeParametersExternalCommand); } }

        public override HashSet<string> JournalDataKeys { get; } = new HashSet<string> { KeySharedFile };
    }
}
