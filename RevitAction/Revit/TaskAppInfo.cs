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

        public string Namespace
        {
            get { return GetType().Namespace; }
        }

        public string FullClassName
        {
            get { return $"{Namespace}.{nameof(TaskApp)}"; }
        }

        public string VendorId { get; } = DefaultVendorId;
    }
}
