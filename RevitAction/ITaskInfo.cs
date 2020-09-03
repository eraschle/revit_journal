namespace RevitAction
{
    public interface ITaskInfo
    {
        string AssemblyPath { get; set; }

        string Namespace { get; }

        string FullClassName { get; }

        string VendorId { get; }
    }
}
