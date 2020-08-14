using DataSource.Model.FileSystem;
using System.Collections.Generic;
using System.Linq;

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

        public bool IsSaveAction { get; protected set; } = false;

        public bool MakeChanges { get; protected set; } = false;

        public bool HasParameter(string name, out IActionParameter parameter)
        {
            parameter = null;
            if (HasParameters)
            {
                parameter = Parameters.FirstOrDefault(par => par.Name == name);
            }
            return parameter != null;
        }

        public bool HasParameter(string name)
        {
            return HasParameter(name, out _);
        }

        public virtual void PreTask(RevitFamily family) { }

        public virtual void PostTask(RevitFamily family) { }

        public virtual void SetLibraryRoot(string libraryRoot) { }
    }
}