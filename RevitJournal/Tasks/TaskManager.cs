using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitJournal.Helper;
using RevitCommand.Families;
using RevitJournal.Journal.Execution;
using RevitJournal.Revit.Commands;
using RevitJournal.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace RevitJournal.Journal
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
            if(RootDirectory is null || 
                RootDirectory.FullPath.Equals(contentDirectory, StringComparison.CurrentCulture) == false)
            {
                RootDirectory = new RevitDirectory(null, contentDirectory);
            }
            return RootDirectory;
        }

        public static int MinParallelProcess { get; } = 1;

        public static int MaxParallelProcess { get; } = Environment.ProcessorCount;

        public string JournalDirectory { get; set; } = string.Empty;


        private readonly object JournalTaskRunnersLock = new object();

        public int ParallelProcess { get; set; } = MaxParallelProcess / 2;

        private IList<JournalTaskRunner> JournalTaskRunners { get; } = new List<JournalTaskRunner>();

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
            JournalTaskRunners.Clear();
        }

        public int JournalTaskCount { get { return RevitTasks.Count; } }

        public int JournalTaskExecutedCount { get; private set; } = 0;

        public void CreateJournalTaskRunner(IProgress<JournalResult> progress)
        {
            const int status = ResultStatus.Waiting;
            foreach (var journalTask in RevitTasks)
            {
                var runner = new JournalTaskRunner(journalTask, progress);
                runner.Report(status);
                JournalTaskRunners.Add(runner);
            }
        }

        public void CleanJournalTaskRunner()
        {
            JournalTaskRunners.Clear();
        }

        private void CreateAddinFile()
        {
            var commandDatas = new HashSet<IRevitExternalCommandData>();

            foreach (var journalTask in RevitTasks)
            {
                if (journalTask.HasCommands(out var actionCommands) == false) { continue; }

                foreach (var command in actionCommands)
                {
                    //commandDatas.Add(command.CommandData);
                }
            }
            foreach (var commandData in commandDatas)
            {
                FileCreationManager.CreateAddinFile(JournalDirectory, commandData);
            }
        }

        public async Task ExecuteAllTaskAsync(CancellationToken cancellation)
        {
            if (JournalTaskRunners.Count == 0) { throw new ArgumentException("No Runner are created"); }

            CreateAddinFile();
            var creator = new JournalRevitCreator(JournalDirectory);
            var creatorTask = creator.CreatorTask(TimeSpan.FromMilliseconds(1111));
            var runnerTask = RunTasks(creator, cancellation);
            await Task.WhenAny(runnerTask, creatorTask);
            creator.WatchingJournals = false;
        }

        private void SetRunnerStatus(int status)
        {
            foreach (var runner in JournalTaskRunners)
            {
                runner.Report(status);
            }
        }

        private Task RunTasks(JournalRevitCreator creator, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                var runningTasks = CreateRunningTasks(creator, cancellation);
                while (runningTasks.Count > 0)
                {
                    var finished = Task.WhenAny(runningTasks.ToArray()).Result;
                    if (cancellation.IsCancellationRequested)
                    {
                        SetRunnerStatus(ResultStatus.Cancel);
                        break;
                    }
                    JournalTaskExecutedCount++;
                    if (JournalTaskRunners.Count > 0)
                    {
                        var task = NextJournalTask(creator, cancellation);
                        runningTasks.Add(task);
                    }
                    runningTasks.Remove(finished);
                }
            });
        }

        private IList<Task> CreateRunningTasks(JournalRevitCreator creator, CancellationToken cancellation)
        {
            var max = Math.Min(ParallelProcess, JournalTaskRunners.Count);
            var runningTasks = new List<Task>();

            int count = 0;
            while (count < max)
            {
                var task = NextJournalTask(creator, cancellation);
                runningTasks.Add(task);
                count++;
            }
            return runningTasks;
        }

        private Task NextJournalTask(JournalRevitCreator creator, CancellationToken cancellation)
        {
            var journalModel = JournalTaskRunners[0];
            JournalTaskRunners.Remove(journalModel);
            return journalModel.CreateTask(creator, cancellation);
        }
    }
}
