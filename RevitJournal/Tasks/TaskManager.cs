using RevitAction.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RevitJournal.Revit.Addin;
using RevitJournal.Tasks.Options;
using RevitJournal.Tasks.Actions;
using RevitAction.Report.Message;
using RevitAction.Report;
using RevitJournal.Report;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using RevitAction;

namespace RevitJournal.Tasks
{
    public partial class TaskManager
    {
        private ReportServer<TaskUnitOfWork> ReportServer { get; set; } = new ReportServer<TaskUnitOfWork>();

        public Queue<TaskUnitOfWork> TaskQueue { get; } = new Queue<TaskUnitOfWork>();

        public IList<TaskUnitOfWork> UnitOfWorks { get; } = new List<TaskUnitOfWork>();

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
                DisconnectAction = ReportServer.DisconnectClient
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
                unitOfWork.Status.SetStatus(status);
                progress.Report(unitOfWork);
            }
        }

        public Task ExecuteTasks(TaskOptions options, IProgress<TaskUnitOfWork> progress, CancellationToken cancellation)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }
            if (progress is null) { throw new ArgumentNullException(nameof(progress)); }

            CreateAddinFile(options);
            SetWaitingStatus(progress);
            return Task.Run(() => { RunTasks(options, progress, cancellation); });
        }

        public void StartServer(TaskOptions options, IProgress<TaskUnitOfWork> progress)
        {
            ReportServer.SetFindReport(ByTaskId);
            ReportServer.SetProgress(progress);
            ReportServer.StartListening(options);
        }

        public void StopServer()
        {
            ReportServer.StopListening();
        }

        private void RunTasks(TaskOptions options, IProgress<TaskUnitOfWork> progress, CancellationToken cancellation)
        {
            var runningTasks = CreateTasks(options, progress, cancellation);
            while (runningTasks.Count > 0)
            {
                var finished = Task.WhenAny(runningTasks.ToArray()).Result;
                runningTasks.Remove(finished);
                if (cancellation.IsCancellationRequested)
                {
                    CancelTasks(progress);
                    TaskQueue.Clear();
                    runningTasks.Clear();
                }
                if (TaskQueue.Count > 0)
                {
                    var nextTask = GetNextTask(progress, cancellation);
                    runningTasks.Add(nextTask);
                }
            }
        }

        private void CancelTasks(IProgress<TaskUnitOfWork> progress)
        {
            if(TaskQueue.Count == 0) { return; }

            foreach (var nextTask in TaskQueue)
            {
                nextTask.KillProcess();
                nextTask.ReportStatus(progress, TaskAppStatus.Cancel);
            }
        }

        private ICollection<Task> CreateTasks(TaskOptions options, IProgress<TaskUnitOfWork> progress, CancellationToken cancellation)
        {
            var max = Math.Min(options.Parallel.ParallelProcesses, TaskQueue.Count);
            var runningTasks = new List<Task>();

            while (runningTasks.Count < max)
            {
                var task = GetNextTask(progress, cancellation);
                runningTasks.Add(task);
            }
            return runningTasks;
        }

        private Task GetNextTask(IProgress<TaskUnitOfWork> progress, CancellationToken cancellation)
        {
            var unitOfWork = TaskQueue.Dequeue();
            return unitOfWork.CreateTask(progress, cancellation);
        }
    }
}
