using DataSource.Model.FileSystem;
using System;
using System.Collections.Generic;

namespace RevitAction.Action
{
    public abstract class ATaskAction : ITaskAction
    {
        protected ATaskAction(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public IList<IActionParameter> Parameters { get; } = new List<IActionParameter>();

        protected void AddParameter(IActionParameter parameter)
        {
            if (parameter is null || Parameters.Contains(parameter)) { return; }

            Parameters.Add(parameter);
        }

        public bool HasParameters
        {
            get { return Parameters != null && Parameters.Count > 0; }
        }

        public bool MakeChanges { get; protected set; } = false;

        public abstract Guid Id { get; }

        public virtual bool DependsOn(ITaskAction action)
        {
            return false;
        }

        public virtual void PreTask(RevitFamily family) { }

        public virtual void SetLibraryRoot(string libraryRoot) { }

        public override bool Equals(object obj)
        {
            return obj is ITaskAction action &&
                   Name == action.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}