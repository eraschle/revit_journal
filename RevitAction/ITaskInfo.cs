namespace RevitAction
{
    public interface ITaskInfo
    {
        string AssemblyPath { get; set; }

        string TaskNamespace { get; }

        string FullClassName { get; }

        string VendorId { get; }
    }
}
