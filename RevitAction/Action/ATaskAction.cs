using DataSource.Model.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Utilities.System;

namespace RevitAction.Action
{
    public abstract class ATaskAction : ITaskAction, IEquatable<ATaskAction>
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

        public abstract void PreTask(RevitFamily family);

        public abstract void SetLibraryRoot(string libraryRoot);

        public int CompareTo(ITaskAction other)
        {
            var compare = 1;
            if(other is object)
            {
                compare = StringUtils.Compare(Name, other.Name);
            }
            return compare;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ATaskAction);
        }

        public bool Equals(ATaskAction other)
        {
            return other != null &&
                   ActionId.Equals(other.ActionId);
        }

        public override int GetHashCode()
        {
            return -848256190 + ActionId.GetHashCode();
        }

        public static bool operator ==(ATaskAction left, ATaskAction right)
        {
            return EqualityComparer<ATaskAction>.Default.Equals(left, right);
        }

        public static bool operator !=(ATaskAction left, ATaskAction right)
        {
            return !(left == right);
        }
    }
}