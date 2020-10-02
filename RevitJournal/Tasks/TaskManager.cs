using RevitAction.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RevitJournal.Revit.Addin;
using RevitJournal.Tasks.Options;
using RevitJournal.Tasks.Actions;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using RevitAction;
using RevitJournal.Report.Network;
using RevitAction.Report;

namespace RevitJournal.Tasks
{
    public class TaskManager
    {
        private object NextTaskLock = new object();

        private ReportServer ReportServer { get; set; } = new ReportServer();

        private ClientController ClientController { get; set; } = new ClientController();

        public Queue<TaskUnitOfWork> TaskQueue { get; } = new Queue<TaskUnitOfWork>();

        public IList<TaskUnitOfWork> UnitOfWorks { get; } = new List<TaskUnitOfWork>();

        private List<ITaskAction> TaskActions { get; } = new List<ITaskAction>();

        public IProgress<TaskUnitOfWork> Progress { get; set; }

        public bool HasTasks
        {
            get { return UnitOfWorks.Count > 0; }
        }

        public static bool IsExecutable(RevitFamily family, TaskOptions options)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            return (options.UseMetadata && IsRevitInstalled(family, out _))
                || ((family.AreMetadataRepairable || family.HasValidMetadata) && options.UseMetadata == false);
        }

        public static bool IsRevitInstalled(RevitFamily family, out RevitApp revit)
        {
            return family is null
                ? throw new ArgumentNullException(nameof(family))
                : family.Metadata.HasProduct(out revit);
        }

        public void SetTaskActions(IEnumerable<ITaskAction> taskActions)
        {
            TaskActions.Clear();
            TaskActions.AddRange(taskActions);
        }

        internal ActionManager GetActionManager()
        {
            var manager = new ActionManager();
            var dialogActions = TaskActions.Select(task => new TaskDialogAction(task.ActionId, task.Name, task.DialogHandlers));
            manager.AddTaskActions(dialogActions);
            return manager;
        }

        public void AddTask(RevitTask task, TaskOptions options)
        {
            if (task is null) { return; }

            task.AddActions(TaskActions);
            var unitOfWork = new TaskUnitOfWork(task, options)
            {
                DisconnectAction = ClientController.Remove
            };
            UnitOfWorks.Add(unitOfWork);
            TaskQueue.Enqueue(unitOfWork);
        }

        public void ClearTasks()
        {
            UnitOfWorks.Clear();
            TaskQueue.Clear();
            TaskActions.Clear();
        }

        private void CreateAddinFile(TaskOptions options)
        {
            var commands = new HashSet<ITaskActionCommand>();
            foreach (var task in UnitOfWorks.Select(uow => uow.Task))
            {
                if (task.HasCommands(out var actionCommands) == false) { continue; }

                foreach (var command in actionCommands)
                {
                    commands.Add(command);
                }
            }

            var workingDirectory = options.GetJournalWorking();
            AddinManager.CreateAppManifest(workingDirectory, ExternalAction.GetTaskApp());
            foreach (var command in commands)
            {
                AddinManager.CreateManifest(workingDirectory, command.TaskInfo);
            }
        }

        internal TaskUnitOfWork ByTaskId(string familyPath)
        {
            return UnitOfWorks.FirstOrDefault(uow => uow.TaskId == familyPath);
        }

        public void SetProgress(IProgress<TaskUnitOfWork> progress)
        {
            foreach (var unitOfWork in UnitOfWorks)
            {
                unitOfWork.Progress = progress;
                unitOfWork.ReportStatus(TaskAppStatus.Waiting);
            }
        }

        public Task ExecuteTasks(TaskOptions options, CancellationToken cancellation)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            CreateAddinFile(options);
            return Task.Run(() => { RunTasks(options, cancellation); });
        }

        public void StartServer(TaskOptions options)
        {
            ClientController.TaskManager = this;
            ReportServer.AddClient = ClientController.AddClient;
            ReportServer.StartListening(options);
        }

        public void StopServer()
        {
            ClientController.RemoveClients();
            ReportServer.StopListening();
        }

        private void RunTasks(TaskOptions options, CancellationToken cancellation)
        {
            var runningTasks = CreateTasks(options, cancellation);
            while (runningTasks.Count > 0)
            {
                var finished = Task.WhenAny(runningTasks.ToArray()).Result;
                if (cancellation.IsCancellationRequested)
                {
                    while (TaskQueue.Count > 0)
                    {
                        var task = TaskQueue.Dequeue();
                        task.CancelProcess();
                    }
                }
                if (TaskQueue.Count > 0)
                {
                    var nextTask = GetNextTask(cancellation);
                    runningTasks.Add(nextTask);
                }
                runningTasks.Remove(finished);
            }
        }

        private ICollection<Task> CreateTasks(TaskOptions options, CancellationToken cancellation)
        {
            var max = Math.Min(options.Parallel.ParallelProcesses, TaskQueue.Count);
            var runningTasks = new List<Task>();

            while (runningTasks.Count < max)
            {
                var task = GetNextTask(cancellation);
                runningTasks.Add(task);
            }
            return runningTasks;
        }

        private Task GetNextTask(CancellationToken cancellation)
        {
            lock (NextTaskLock)
            {
                var unitOfWork = TaskQueue.Dequeue();
                var nextTask = unitOfWork.CreateTask(cancellation);
                Task.Delay(TimeSpan.FromMilliseconds(500)).Wait();
                return nextTask;
            }
        }
    }
}
