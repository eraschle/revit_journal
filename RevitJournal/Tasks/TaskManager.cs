using RevitAction.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RevitJournal.Revit.Addin;
using RevitJournal.Tasks.Report;
using RevitAction.Report;
using RevitJournal.Tasks.Options;
using RevitJournal.Tasks.Actions;

namespace RevitJournal.Tasks
{
    public partial class TaskManager
    {
        private ReportServer ReportServer { get; set; } = new ReportServer();

        public Queue<TaskUnitOfWork> TaskQueue { get; } = new Queue<TaskUnitOfWork>();

        public IList<TaskUnitOfWork> UnitOfWorks { get; } = new List<TaskUnitOfWork>();

        public bool HasTasks
        {
            get { return UnitOfWorks.Count > 0; }
        }

        public void AddTask(RevitTask task, TaskOptions options)
        {
            if (task is null) { return; }

            var status = ReportStatus.Initial;
            var unitOfWork = new TaskUnitOfWork(task, options);
            unitOfWork.SetStatus(status);
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
                AddinManager.CreateManifest(journal, command);
            }
        }

        internal TaskUnitOfWork ByFamilyPath(string familyPath)
        {
            return UnitOfWorks.FirstOrDefault(uow => uow.TaskId == familyPath);
        }

        private void SetWaitingStatus()
        {
            var status = ReportStatus.Waiting;
            foreach (var unitOfWork in UnitOfWorks)
            {
                unitOfWork.SetStatus(status);
            }
        }

        public Task ExecuteTasks(TaskOptions options, CancellationToken cancellation)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            CreateAddinFile(options);
            SetWaitingStatus();
            return Task.Run(()=> { RunTasks(options, cancellation); });
        }

        public void StartServer(TaskOptions options)
        {
            ReportServer.FindFunc = ByFamilyPath;
            ReportServer.StartListening(options);
        }

        public void StopServer()
        {
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
                    foreach (var nextTask in TaskQueue)
                    {
                        nextTask.CancelProcess();
                    }
                    break;
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
