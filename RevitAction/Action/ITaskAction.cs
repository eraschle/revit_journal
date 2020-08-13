using DataSource.Model.FileSystem;
using RevitAction.Action.Revit;
using System.Collections.Generic;

namespace RevitAction.Action
{
    public interface ITaskAction
    {
        string Name { get; }

        IList<IActionParameter> Parameters { get; }

        bool HasParameters { get; }

        bool HasParameter(string name, out IActionParameter parameter);

        bool HasParameter(string name);

        bool MakeChanges { get; }

        bool SaveAction { get; }

        void PreTask(RevitFamily revit);

        IRevitTaskAction GetRevitAction();

        void SetRootDirectory(string rootDirectory);
    }
}
