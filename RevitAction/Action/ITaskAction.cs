using DataSource.Models.FileSystem;
using System;
using System.Collections.Generic;

namespace RevitAction.Action
{
    public interface ITaskAction : IComparable<ITaskAction>
    {
        Guid ActionId { get; }

        string Name { get; }

        IList<IActionParameter> Parameters { get; }

        bool MakeChanges { get; }

        void SetLibraryRoot(string libraryRoot);

        void PreTask(RevitFamilyFile family);

        ICollection<DialogHandler> DialogHandlers { get; }
    }
}
