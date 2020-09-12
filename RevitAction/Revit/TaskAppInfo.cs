using System;

namespace RevitAction.Revit
{
    public class TaskAppInfo : ITaskAppInfo
    {
        public Guid Id { get; } = new Guid("5aa93805-8e2a-4b8a-8087-94556cebe3b7");

        public string VendorId { get; set; }

        public string TaskNamespace
        {
            get { return GetType().Namespace; }
        }

        public string ClassName { get; } = nameof(TaskApp);

        public string FullClassName
        {
            get { return $"{TaskNamespace}.{ClassName}"; }
        }


        public string AssemblyPath
        {
            get { return GetType().Assembly.Location; }
            set { }
        }

    }
}
