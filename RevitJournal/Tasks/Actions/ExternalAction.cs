using RevitAction;
using RevitAction.Action;
using RevitJournal.Revit.Journal.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RevitJournal.Tasks.Actions
{
    public class ExternalAction
    {
        private static readonly ExternalActionDataSource dataSource = new ExternalActionDataSource();

        public static ITaskAppInfo GetTaskApp()
        {
            return GetTaskAppInfo(typeof(ITaskInfo).Assembly);
        }

        private static ITaskAppInfo GetTaskAppInfo(Assembly assemby)
        {
            return assemby.GetLoadableTypes()
                          .Where(IsTaskApp)
                          .Select(action => CreateApp(action))
                          .FirstOrDefault(action => action != null);
        }

        private static bool IsTaskApp(Type other)
        {
            var actionType = typeof(ITaskAppInfo);
            return actionType.IsAssignableFrom(other)
                && other.IsInterface == false
                && other.IsAbstract == false;
        }

        private static ITaskAppInfo CreateApp(Type actionType)
        {
            var action = Activator.CreateInstance(actionType, false) as ITaskAppInfo;
            return action;
        }

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

        private static IEnumerable<ExternalAction> GetExternalActions(string directory)
        {
            var files = Directory.GetFiles(directory, $"*.{ExternalActionFile.FileExtension}")
                                 .Select(path => new ExternalActionFile { FullPath = path });
            var externalActions = files.Select(path => dataSource.Read(path));

#if DEBUG
            externalActions = GetDebugFiles();
#endif
            return externalActions;
        }

        public string Assembly { get; set; }


        public IEnumerable<ITaskAction> GetTaskActions()
        {
            var assembly = System.Reflection.Assembly.LoadFile(Assembly);
            return GetActions(assembly);
        }

        private IEnumerable<ITaskAction> GetActions(Assembly assemby)
        {
            return assemby.GetLoadableTypes()
                          .Where(IsTaskAction)
                          .Select(action => CreateAction(action))
                          .Where(action => action != null);
        }

        private ITaskAction CreateAction(Type actionType)
        {
            var action = Activator.CreateInstance(actionType, false) as ITaskAction;
            if (action is ITaskActionCommand command)
            {
                command.AssemblyPath = Assembly;
            }
            return action;
        }

        private bool IsTaskAction(Type other)
        {
            var actionType = typeof(ITaskAction);
            var commandType = typeof(ITaskActionCommand);
            return (actionType.IsAssignableFrom(other) || commandType.IsAssignableFrom(other))
                && other.IsInterface == false
                && other.IsAbstract == false;
        }

        internal static IEnumerable<ExternalAction> GetDebugFiles()
        {
            var debugDir = Path.GetDirectoryName(typeof(ExternalAction).Assembly.Location);
            var binDir = Path.GetDirectoryName(debugDir);
            var projDir = Path.GetDirectoryName(binDir);
            var solutionDir = Path.GetDirectoryName(projDir);
            var debugAction = Path.Combine(solutionDir, "RevitCommand", "bin", "debug", "RevitCommand.dll");
            return new List<ExternalAction> { new ExternalAction { Assembly = debugAction } };
        }
    }
}
