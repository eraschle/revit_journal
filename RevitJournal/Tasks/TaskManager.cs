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
using RevitJournal.Revit;

namespace RevitJournal.Tasks
{
    public partial class TaskManager
    {
        private ReportServer ReportServer { get; set; } = new ReportServer();

        public Queue<RevitTask> TaskQueue { get; } = new Queue<RevitTask>();

        public IList<RevitTask> RevitTasks { get; } = new List<RevitTask>();

        public bool HasRevitTasks
        {
            get { return RevitTasks.Count > 0; }
        }

        public void AddTask(RevitTask task)
        {
            if (task is null) { return; }

            RevitTasks.Add(task);
        }

        public void ClearTasks()
        {
            RevitTasks.Clear();
            CleanTaskRunner();
        }

        public int TaskCount
        {
            get { return RevitTasks.Count; }
        }


        public void CreateTaskQueue(Progress<TaskReport> progress)
        {
            const int status = ReportStatus.Waiting;
            foreach (var task in RevitTasks)
            {
                task.SetStatus(status);
                task.Progress = progress;
                TaskQueue.Enqueue(task);
            }
        }

        public void CleanTaskRunner()
        {
            TaskQueue.Clear();
        }

        private void CreateAddinFile(TaskOptions options)
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
            AddinManager.CreateAppManifest(journal, ExternalAction.GetTaskApp());
            foreach (var command in commands)
            {
                AddinManager.CreateManifest(journal, command);
            }
        }

        internal RevitTask ByFamilyPath(string familyPath)
        {
            return RevitTasks.FirstOrDefault(task => task.TaskId == familyPath);
        }

        public async Task ExecuteTasks(TaskOptions options, CancellationToken cancellation)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            CreateAddinFile(options);
            await RunTasks(options, cancellation).ConfigureAwait(false);
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

        private Task RunTasks(TaskOptions options, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                var runningTasks = CreateTasks(options, cancellation);
                while (runningTasks.Count > 0)
                {
                    var finished = Task.WhenAny(runningTasks.ToArray());
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
                        var nextTask = GetNextTask(options, cancellation);
                        runningTasks.Add(nextTask);
                    }
                    runningTasks.Remove(finished);
                }
            });
        }

        private ICollection<Task> CreateTasks(TaskOptions options, CancellationToken cancellation)
        {
            var max = Math.Min(options.Parallel.ParallelProcesses, TaskQueue.Count);
            var runningTasks = new List<Task>();

            var count = 0;
            while (count < max)
            {
                var task = GetNextTask(options, cancellation);
                runningTasks.Add(task);
                count++;
            }
            return runningTasks;
        }

        private Task GetNextTask(TaskOptions options, CancellationToken cancellation)
        {
            var revitTask = TaskQueue.Dequeue();
            revitTask.CreateJournalProcess(options);
            revitTask.PreExecution(options.Backup);
            return CreateTask(revitTask, options, cancellation);
        }

        private async Task<bool> CreateTask(RevitTask task, TaskOptions options, CancellationToken cancellation)
        {
            var normalExit = false;
            using (var taskCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
            {
                taskCancellation.Token.Register(task.KillProcess);
                using (task.Process = new RevitProcess(options.Arguments))
                {
                    var journal = task.JournalTask;
                    normalExit = await task.Process.RunTaskAsync(journal, taskCancellation.Token)
                                                   .ConfigureAwait(false);
                }
            }
            return normalExit;
        }

    }
}
