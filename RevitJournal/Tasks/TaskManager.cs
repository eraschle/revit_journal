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

namespace RevitJournal.Tasks
{
    public class TaskManager
    {
        private ReportServer<TaskUnitOfWork> ReportServer { get; set; } = new ReportServer<TaskUnitOfWork>();

        private ClientController<TaskUnitOfWork> ClientController { get; set; } = new ClientController<TaskUnitOfWork>();

        public Queue<TaskUnitOfWork> TaskQueue { get; } = new Queue<TaskUnitOfWork>();

        public IList<TaskUnitOfWork> UnitOfWorks { get; } = new List<TaskUnitOfWork>();

        public IProgress<TaskUnitOfWork> Progress { get; set; }

        public bool HasTasks
        {
            get { return UnitOfWorks.Count > 0; }
        }

        public static bool IsExecutable(RevitFamily family, TaskOptions options)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            return family.HasRepairableMetadata
                || (family.HasValidMetadata && options.UseMetadata);
        }

        public static bool IsRevitInstalled(RevitFamily family, out RevitApp revit)
        {
            return family is null
                ? throw new ArgumentNullException(nameof(family))
                : family.Metadata.HasProduct(out revit);
        }

        public void AddTask(RevitTask task, TaskOptions options)
        {
            if (task is null) { return; }

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
            CleanTaskRunner();
        }

        public void CleanTaskRunner()
        {
            TaskQueue.Clear();
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

            var journal = options.JournalDirectory;
            AddinManager.CreateAppManifest(journal, ExternalAction.GetTaskApp());
            foreach (var command in commands)
            {
                AddinManager.CreateManifest(journal, command.TaskInfo);
            }
        }

        internal TaskUnitOfWork ByTaskId(string familyPath)
        {
            return UnitOfWorks.FirstOrDefault(uow => uow.TaskId == familyPath);
        }

        private void SetWaitingStatus(IProgress<TaskUnitOfWork> progress)
        {
            var status = TaskAppStatus.Waiting;
            foreach (var unitOfWork in UnitOfWorks)
            {
                unitOfWork.Progress = progress;
                unitOfWork.ReportStatus(status);
            }
        }

        public Task ExecuteTasks(TaskOptions options, IProgress<TaskUnitOfWork> progress, CancellationToken cancellation)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }
            if (progress is null) { throw new ArgumentNullException(nameof(progress)); }

            CreateAddinFile(options);
            SetWaitingStatus(progress);
            return Task.Run(() => { RunTasks(options, cancellation); });
        }

        public void StartServer(TaskOptions options)
        {
            ClientController.FindReport = ByTaskId;
            ReportServer.Clients = ClientController;
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
            var unitOfWork = TaskQueue.Dequeue();
            return unitOfWork.CreateTask(cancellation);
        }
    }
}
