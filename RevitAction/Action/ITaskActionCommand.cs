using System;

namespace RevitAction.Action
{
    public interface ITaskActionCommand : ITaskAction
    {
        Guid AddinId { get; }

        string AssemblyPath { get; set; }

        string Namespace { get; }

        string FullClassName { get; }

        string VendorId { get; }
    }
}
