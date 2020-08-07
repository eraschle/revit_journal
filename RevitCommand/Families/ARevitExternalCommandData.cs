using System;
using System.Collections.Generic;

namespace RevitCommand.Families
{
    public abstract class ARevitExternalCommandData : IRevitExternalCommandData, IEquatable<ARevitExternalCommandData>
    {
        public abstract Guid AddinId { get; }
        public abstract string CommandName { get; }

        public string Namespace { get { return CommandDataType.Namespace; } }

        public string FullClassName { get { return $"{Namespace}.{ExternalCommandName}"; } }

        public string VendorId { get; } = "AmWaDev";

        public virtual HashSet<string> JournalDataKeys { get; } = new HashSet<string>();

        protected abstract Type CommandDataType { get; }

        protected abstract string ExternalCommandName { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ARevitExternalCommandData);
        }

        public bool Equals(ARevitExternalCommandData other)
        {
            return other != null &&
                   AddinId.Equals(other.AddinId);
        }

        public override int GetHashCode()
        {
            return -1904336490 + AddinId.GetHashCode();
        }
    }
}
