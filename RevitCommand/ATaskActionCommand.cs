using RevitAction.Action;
using System;

namespace RevitCommand
{
    public abstract class ATaskActionCommand : ATaskAction, ITaskActionCommand
    {
        protected ATaskActionCommand(string name) : base(name) { }

        public abstract Guid AddinId { get; }

        public abstract string Namespace { get; }

        public string FullClassName
        {
            get { return $"{Namespace}.{ExternalCommandName}"; }
        }

        protected abstract string ExternalCommandName { get; }

        public string VendorId { get; } = "RascerDev";

        public string AssemblyPath { get; set; }
    }
}