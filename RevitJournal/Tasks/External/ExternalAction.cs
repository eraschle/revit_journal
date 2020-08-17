using RevitAction.Action;
using RevitJournal.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RevitJournal.Tasks.External
{
    public class ExternalAction
    {
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
