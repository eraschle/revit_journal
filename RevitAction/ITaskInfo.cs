using System;

namespace RevitAction
{
    public interface ITaskInfo
    {
        Guid Id { get; }

        string VendorId { get; }

        string TaskNamespace { get; }

        string ClassName { get; }

        string FullClassName { get; }

        string AssemblyPath { get; set; }
    }
}
