using System;
using System.Collections.Generic;

namespace RevitCommand.Families.ImageExport
{
    public class ThreeDImageExportCommandData : ARevitExternalCommandData
    {
        public const string KeyBackground = "Background";

        public override Guid AddinId { get; } = new Guid("d62ba092-cf93-4906-90d8-1948d2dd67c5");

        public override string CommandName { get; } = "3D Image Export";

        protected override Type CommandDataType { get { return GetType(); } }

        protected override string ExternalCommandName { get { return nameof(ThreeDImageExportExternalCommand); } }

        public override HashSet<string> JournalDataKeys { get; } = new HashSet<string> { KeyBackground };
    }
}
