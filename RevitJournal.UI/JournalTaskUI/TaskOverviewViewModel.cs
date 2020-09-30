using RevitJournal.Tasks;
using RevitJournal.Tasks.Options;
using RevitJournalUI.Tasks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI
{
    public class TaskOverviewViewModel : ANotifyPropertyChangedModel
    {
        public Progress<TaskUnitOfWork> Progress { get; set; } = new Progress<TaskUnitOfWork>();

        private const string PrefixExecutedTask = "Executed Tasks";

        private readonly DispatcherTimer timer;

        public TaskOverviewViewModel()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        }

        public ObservableCollection<TaskViewModel> TaskModels { get; } = new ObservableCollection<TaskViewModel>();

        private int minTasks = 1;
        public int MinTasks
        {
            get { return minTasks; }
            set
            {
                if (minTasks == value) { return; }

                minTasks = value;
                NotifyPropertyChanged();
            }
        }

        private int maxTasks = 0;
        public int MaxTasks
        {
            get { return maxTasks; }
            set
            {
                if (maxTasks == value) { return; }

                maxTasks = value;
                NotifyPropertyChanged();
            }
        }

        public int ExecutedTasks { get; private set; }

        private string executedTasksText = string.Empty;
        public string ExecutedTasksText
        {
            get { return executedTasksText; }
            set
            {
                if (StringUtils.Equals(executedTasksText, value)) { return; }

                executedTasksText = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ExecutedTasks));
            }
        }

        internal void AddEvents()
        {
            foreach (var viewModel in TaskModels)
            {
                viewModel.AddTimer(timer);
                viewModel.AddProgessEvent(Progress);
            }
            Progress.ProgressChanged += Progress_ProgressChanged;
            timer.Start();
        }

        internal void RemoveEvents()
        {
            timer.Stop();
            foreach (var viewModel in TaskModels)
            {
                viewModel.RemoveTimer(timer);
                viewModel.RemoveProgessEvent(Progress);
            }
            Progress.ProgressChanged -= Progress_ProgressChanged;
        }

        internal void Update(TaskManager manager, TaskOptions options)
        {
            if (manager is null || manager.HasTasks == false) { return; }

            TaskModels.Clear();
            MaxTasks = 0;
            var formats = new string[] { DateUtils.Minute, DateUtils.Seconds };
            var totalTime = DateUtils.AsString(options.Arguments.Timeout, Constant.Point, formats);
            foreach (var unitOfWork in manager.UnitOfWorks)
            {
                MaxTasks += 1;
                var viewModel = new TaskViewModel
                {
                    TaskUoW = unitOfWork,
                    AllExecutedFunc = AllTaskExecuted,
                    TotalTime = totalTime
                };
                TaskModels.Add(viewModel);
            }

            ExecutedTasks = 0;
            SetExecutedTasks();
        }

        private bool AllTaskExecuted()
        {
            return ExecutedTasks >= MaxTasks;
        }

        private void Progress_ProgressChanged(object sender, TaskUnitOfWork task)
        {
            if (task.Status.IsExecuted == false) { return; }

            var viewModel = TaskModels.FirstOrDefault(mdl => mdl.TaskUoW.Equals(task));
            if (viewModel is null) { return; }

            SetExecutedTasks();
            task.Cleanup();
            viewModel.RemoveTimer(timer);
            viewModel.RemoveProgessEvent(Progress);
        }

        private void SetExecutedTasks()
        {
            var executed = TaskModels.Count(model => model.TaskUoW.Status.IsExecuted);
            if(executed == ExecutedTasks) { return; }

            ExecutedTasks = executed;
            ExecutedTasksText = string.Join(Constant.Space, PrefixExecutedTask, ExecutedTasks, Constant.SlashChar, MaxTasks) ;
        }
    }
}
