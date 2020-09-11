using System;

namespace RevitAction.Action
{
    public class TaskActionInfo<TAction> : ITaskInfo where TAction : ITaskAction
    {
        public TaskActionInfo(string typeName)
        {
            ClassName = typeName;
        }

        public string AssemblyPath { get; set; } = typeof(TAction).Assembly.Location;

        public Guid Id { get; set; }

        public string TaskNamespace
        {
            get { return typeof(TAction).Namespace; }
        }

        public string ClassName { get; }

        public string FullClassName
        {
            get { return $"{TaskNamespace}.{ClassName}"; }
        }

        public string VendorId { get; set; } = "RascerDev";

    }
}
