using DataSource.Model.FileSystem;
using RevitAction.Action.Revit;
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

        public bool MakeChanges { get; protected set; } = false;

        public abstract IRevitTaskAction GetRevitAction();

        public bool SaveAction { get; protected set; } = false;

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

        public void ChangeParameter(string name, IActionParameter parameter)
        {
            if (HasParameter(name, out var oldParameter) == false) { return; }

            var index = Parameters.IndexOf(oldParameter);
            Parameters.RemoveAt(index);
            Parameters.Insert(index, parameter);
        }


        public virtual void PreTask(RevitFamily revit) { }

        public virtual void PostTask(object result) { }

        public virtual void SetRootDirectory(string rootDirectory) { }
    }
}