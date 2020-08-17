using DataSource.Model.FileSystem;
using System;
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

        bool DependsOn(ITaskAction action);

        void SetLibraryRoot(string libraryRoot);

        void PreTask(RevitFamily family);

        void PostTask(RevitFamily family);
    }
}
