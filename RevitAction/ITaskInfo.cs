using System;

namespace RevitAction
{
    public interface ITaskInfo
    {
        Guid Id { get; }

        string VendorId { get; set; }

        string TaskNamespace { get; }

        string ClassName { get; }

        string FullClassName { get; }

        string AssemblyPath { get; set; }
    }
}
