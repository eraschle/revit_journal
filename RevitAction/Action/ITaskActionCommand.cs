namespace RevitAction.Action
{
    public interface ITaskActionCommand : ITaskAction
    {
        string AssemblyPath { get; set; }

        string Namespace { get; }

        string FullClassName { get; }

        string VendorId { get; }
    }
}
