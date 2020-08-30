using RevitAction.Action;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using RevitJournal.Tasks.External;
using RevitJournal.Revit.Journal.Command;

namespace RevitJournal.Tasks
{
    public static class TaskActionHelper
    {
        public static IEnumerable<ITaskAction> GetTaskActions(string directory)
        {
            var externalActions = new List<ITaskAction>();
            foreach (var extneral in GetExternalActions(directory))
            {
                externalActions.AddRange(extneral.GetTaskActions());
            }
            externalActions.Sort(new TaskActionComparer());
            externalActions.Insert(0, new DocumentOpenAction());
            externalActions.Add(new DocumentSaveAction());
            externalActions.Add(new DocumentSaveAsAction());
            return externalActions;
        }

        private static readonly ExternalActionDataSource dataSource = new ExternalActionDataSource();

        private static IEnumerable<ExternalAction> GetExternalActions(string directory)
        {
            var files = Directory.GetFiles(directory, $"*.{ExternalActionFile.FileExtension}")
                                 .Select(path => new ExternalActionFile { FullPath = path });
            var externalActions = files.Select(path => dataSource.Read(path));

#if DEBUG
            externalActions = ExternalAction.GetDebugFiles();
#endif
            return externalActions;
        }

    }
}
