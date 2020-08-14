using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitJournal.Helper;
using RevitJournal.Journal.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using RevitJournal.Revit.Addin;
using RevitJournal.Tasks.Options;
using RevitJournal.Journal;

namespace RevitJournal.Tasks
{
    public partial class TaskManager
    {
        private static RevitDirectory RootDirectory;

        public static bool HasRootDirectory(out RevitDirectory directory)
        {
            directory = RootDirectory;
            return directory != null;
        }
        public static RevitDirectory CreateRootDirectory(string contentDirectory)
        {
            if (RootDirectory is null ||
                RootDirectory.FullPath.Equals(contentDirectory, StringComparison.CurrentCulture) == false)
            {
                RootDirectory = new RevitDirectory(null, contentDirectory);
            }
            return RootDirectory;
        }

        private IList<JournalTaskRunner> TaskRunners { get; } = new List<JournalTaskRunner>();

        public IList<RevitTask> RevitTasks { get; } = new List<RevitTask>();

        public bool HasRevitTasks { get { return RevitTasks.Count > 0; } }

        public IEnumerable<ITaskAction> AllTaskActions
        {
            get { return CreateTaskActions(); }
        }

        private List<ITaskAction> CreateTaskActions()
        {
            var taskActions = new List<ITaskAction>();
            foreach (var assemby in AppDomain.CurrentDomain.GetAssemblies())
            {
                var actions = GetActions(assemby);
                taskActions.AddRange(actions);
            }
            taskActions.Sort(new TaskActionComparer());
            return taskActions;
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
            return Activator.CreateInstance(actionType, false) as ITaskAction;
        }

        private bool IsTaskAction(Type other)
        {
            var actionType = typeof(ITaskAction);
            return actionType.IsAssignableFrom(other)
                && other.IsInterface == false
                && other.IsAbstract == false;
        }

        public void AddTask(RevitTask task)
        {
            if (task is null) { return; }

            RevitTasks.Add(task);
        }

        public void ClearTasks()
        {
            RevitTasks.Clear();
            TaskRunners.Clear();
        }

        public int JournalTaskCount { get { return RevitTasks.Count; } }

        public int TaskExecutedCount { get; private set; } = 0;

        public void CreateTaskRunner(IProgress<JournalResult> progress)
        {
            const int status = ResultStatus.Waiting;
            foreach (var journalTask in RevitTasks)
            {
                var runner = new JournalTaskRunner(journalTask, progress);
                runner.Report(status);
                TaskRunners.Add(runner);
            }
        }

        public void CleanTaskRunner()
        {
            TaskRunners.Clear();
        }

        private void CreateAddinFile(CommonOptions options)
        {
            var commands = new HashSet<ITaskActionCommand>();

            foreach (var task in RevitTasks)
            {
                if (task.HasCommands(out var actionCommands) == false) { continue; }

                foreach (var command in actionCommands)
                {
                    commands.Add(command);
                }
            }

            var journal = options.JournalDirectory;
            foreach (var command in commands)
            {
                AddinManager.CreateManifest(journal, command);
            }
        }

        public Task ExecuteTasks(TaskOptions options, CancellationToken cancellation)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            CreateAddinFile(options.Common);
            return RunTasks(options, cancellation);
        }

        private void SetRunnerStatus(int status)
        {
            foreach (var runner in TaskRunners)
            {
                runner.Report(status);
            }
        }

        private Task RunTasks(TaskOptions options, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                var runningTasks = CreateTasks(options, cancellation);
                while (runningTasks.Count > 0)
                {
                    var finished = Task.WhenAny(runningTasks.ToArray()).Result;
                    if (cancellation.IsCancellationRequested)
                    {
                        SetRunnerStatus(ResultStatus.Cancel);
                        break;
                    }
                    TaskExecutedCount++;
                    if (TaskRunners.Count > 0)
                    {
                        var task = NextTask(options, cancellation);
                        runningTasks.Add(task);
                    }
                    runningTasks.Remove(finished);
                }
            });
        }

        private IList<Task> CreateTasks(TaskOptions options, CancellationToken cancellation)
        {
            var max = Math.Min(options.Parallel.ParallelProcesses, TaskRunners.Count);
            var runningTasks = new List<Task>();

            var count = 0;
            while (count < max)
            {
                var task = NextTask(options, cancellation);
                runningTasks.Add(task);
                count++;
            }
            return runningTasks;
        }

        private Task NextTask(TaskOptions options, CancellationToken cancellation)
        {
            var journalModel = TaskRunners[0];
            TaskRunners.Remove(journalModel);
            return journalModel.CreateTask(options, cancellation);
        }
    }
}
