using DataSource.Model.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace RevitAction.Action
{
    public abstract class ATaskAction : ITaskAction
    {
        protected ATaskAction(string name, Guid actionId)
        {
            Name = name;
            ActionId = actionId;
        }

        public Guid ActionId { get; }

        public string Name { get; private set; }

        public IList<IActionParameter> Parameters { get; } = new List<IActionParameter>();

        protected void AddParameter(IActionParameter parameter)
        {
            if (parameter is null || Parameters.Contains(parameter)) { return; }

            Parameters.Add(parameter);
        }

        [JsonIgnore]
        public virtual bool MakeChanges { get; protected set; } = false;

        public ICollection<DialogHandler> DialogHandlers { get; } = new List<DialogHandler>();

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

        public int CompareTo(ITaskAction other)
        {
            return other is null ? 1 : Name.CompareTo(other.Name);
        }

    }
}