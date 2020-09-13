using DataSource.Model.FileSystem;
using RevitAction.Report;
using System;
using System.Collections.Generic;

namespace RevitAction.Action
{
    public interface ITaskAction
    {
        Guid ActionId { get; }

        string Name { get; }

        IList<IActionParameter> Parameters { get; }

        bool HasParameters { get; }

        bool MakeChanges { get; }

        bool DependsOn(ITaskAction action);

        void SetLibraryRoot(string libraryRoot);

        void PreTask(RevitFamily family);

        ICollection<DialogHandler> DialogHandlers { get; }

        bool HasDialogHandlers { get; }
    }
}
