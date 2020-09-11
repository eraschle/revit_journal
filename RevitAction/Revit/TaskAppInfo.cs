using System;

namespace RevitAction.Revit
{
    public class TaskAppInfo : ITaskAppInfo
    {
        public static string DefaultVendorId { get; } = "RascerDev";

        public Guid Id { get { return Guid.Empty; } }

        public string VendorId { get; } = DefaultVendorId;

        public string TaskNamespace
        {
            get { return GetType().Namespace; }
        }

        public string TypeName { get; } = nameof(TaskApp);

        public string FullClassName
        {
            get { return $"{TaskNamespace}.{TypeName}"; }
        }


        public string AssemblyPath
        {
            get { return GetType().Assembly.Location; }
            set { }
        }

    }
}
