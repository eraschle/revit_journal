using DataSource.Model.FileSystem;
using RevitCommand.Families;
using RevitJournal.Journal.Command;
using RevitJournal.Journal.Execution;
using RevitJournal.Revit.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RevitJournal.Journal
{
    public partial class JournalManager
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

        public IList<JournalTask> JournalTasks { get; } = new List<JournalTask>();

        public bool HasJournalTasks { get { return JournalTasks.Count > 0; } }

        private IEnumerable<IJournalCommand> _UserJournalCommands;
        public IEnumerable<IJournalCommand> UserJournalCommands
        {
            get
            {
                if(_UserJournalCommands is null)
                {
                    _UserJournalCommands = FindJournalCommands().Where(cmd => cmd.IsDefault == false);
                }
                return _UserJournalCommands;
            }
        }

        private IEnumerable<IJournalCommand> FindJournalCommands()
        {
            var journalCmdType = typeof(IJournalCommand);
            var journalCommands = new List<IJournalCommand>(
                AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => journalCmdType.IsAssignableFrom(p) && p.IsInterface == false && p.IsAbstract == false)
                .Select(cmdType => Activator.CreateInstance(cmdType, true))
                .Select(cmd => cmd as IJournalCommand)
                .Where(cmd => cmd != null));
            journalCommands.Sort(new JournalCommandComparer());
            return journalCommands;
        }

        public void AddTask(JournalTask task)
        {
            if (task is null) { return; }

            JournalTasks.Add(task);
        }

        public void ClearTasks()
        {
            JournalTasks.Clear();
            JournalTaskRunners.Clear();
        }

        public int JournalTaskCount { get { return JournalTasks.Count; } }

        public int JournalTaskExecutedCount { get; private set; } = 0;

        public void CreateJournalTaskRunner(IProgress<JournalResult> progress)
        {
            const int status = ResultStatus.Waiting;
            foreach (var journalTask in JournalTasks)
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

            foreach (var journalTask in JournalTasks)
            {
                if (journalTask.HasExternalCommands(out var externalCommands) == false) { continue; }

                foreach (var command in externalCommands)
                {
                    commandDatas.Add(command.CommandData);
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
