using System;
using System.Collections.Generic;

namespace RevitCommand.Families
{
    public interface IRevitExternalCommandData
    {
        Guid AddinId { get; }
        
        string CommandName { get; }

        string Namespace { get; }

        string FullClassName { get; }
        
        string VendorId { get; }

        HashSet<string> JournalDataKeys { get; }
    }
}
