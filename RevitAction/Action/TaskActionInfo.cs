using System;

namespace RevitAction.Action
{
    public class TaskActionInfo<TAction, TCommand> : ITaskInfo
        where TAction : ITaskAction
        where TCommand : class
    {
        public string AssemblyPath { get; set; } = typeof(TAction).Assembly.Location;

        public Guid Id { get; set; }

        public string TaskNamespace
        {
            get { return typeof(TAction).Namespace; }
        }

        public string TypeName
        {
            get { return nameof(TCommand); }
        }

        public string FullClassName
        {
            get { return $"{TaskNamespace}.{TypeName}"; }
        }

        public string VendorId { get; set; } = "RascerDev";

    }
}
