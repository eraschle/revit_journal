namespace RevitAction.Revit
{
    public class TaskAppInfo : ITaskAppInfo
    {
        public static string DefaultVendorId { get; } = "RascerDev";

        public string AssemblyPath
        {
            get { return GetType().Assembly.Location; }
            set { }
        }

        public string TaskNamespace
        {
            get { return GetType().Namespace; }
        }

        public string FullClassName
        {
            get { return $"{TaskNamespace}.{nameof(TaskApp)}"; }
        }

        public string VendorId { get; } = DefaultVendorId;
    }
}
