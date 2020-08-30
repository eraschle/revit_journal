using RevitAction.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RevitJournal.Revit.Addin;
using RevitJournal.Journal;
using RevitJournal.Revit;
using RevitJournal.Revit.Report;

namespace RevitJournal.Tasks
{
    public partial class TaskManager
    {
        private ReportServer ReportServer { get; set; } = new ReportServer();

        private Queue<RevitTask> TaskQueue { get; } = new Queue<RevitTask>();

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

        public int ExecutedCount { get; private set; } = 0;

        public void CreateTaskRunner(IProgress<TaskReport> progress)
        {
            const int status = TaskReportStatus.Waiting;
            foreach (var task in RevitTasks)
            {
                task.Report.SetReport(status);
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
            foreach (var command in commands)
            {
                AddinManager.CreateManifest(journal, command);
            }
        }

        internal TaskReport ByFamilyPath(string familyPath)
        {
            var revitTask = RevitTasks.FirstOrDefault(task => task.SourceFile.FullPath == familyPath);
            return revitTask?.Report;
        }

        public Task ExecuteTasks(TaskOptions options, CancellationToken cancellation)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            ReportServer.Manager = this;
            ReportServer.StartListening(options);
            CreateAddinFile(options);
            var allTasks = RunTasks(options, cancellation);
            ReportServer.StopListening();
            return allTasks;
        }

        private Task RunTasks(TaskOptions options, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                var runningTasks = CreateTasks(options, cancellation);
                while (runningTasks.Count > 0)
                {
                    var finished = Task.WhenAny(runningTasks.Keys.ToArray()).Result;
                    var task = runningTasks[finished];
                    if(finished.Result)
                    {
                        task.Report.SetReport(TaskReportStatus.Finish);
                    }
                    else
                    {
                        task.Report.SetReport(TaskReportStatus.Timeout);
                    }
                    task.PostExecution();
                    ExecutedCount++;
                    if (cancellation.IsCancellationRequested)
                    {
                        foreach (var nextTask in TaskQueue)
                        {
                            nextTask.Report.SetReport(TaskReportStatus.Cancel);
                        }
                        break;
                    }
                    if (TaskQueue.Count > 0)
                    {
                        var nextTask = GetNextTask(options, cancellation, out var revitTask);
                        runningTasks.Add(nextTask, revitTask);
                    }
                    runningTasks.Remove(finished);
                }
            });
        }

        private Dictionary<Task<bool>, RevitTask> CreateTasks(TaskOptions options, CancellationToken cancellation)
        {
            var max = Math.Min(options.Parallel.ParallelProcesses, TaskQueue.Count);
            var runningTasks = new Dictionary<Task<bool>, RevitTask>();

            var count = 0;
            while (count < max)
            {
                var task = GetNextTask(options, cancellation, out var revitTask);
                runningTasks.Add(task, revitTask);
                count++;
            }
            return runningTasks;
        }

        private Task<bool> GetNextTask(TaskOptions options, CancellationToken cancellation, out RevitTask revitTask)
        {
            revitTask = TaskQueue.Dequeue();
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
