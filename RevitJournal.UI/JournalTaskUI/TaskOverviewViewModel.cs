using RevitJournal.Tasks;
using RevitJournalUI.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Utilities.UI;

namespace RevitJournalUI.JournalTaskUI
{
    public class TaskOverviewViewModel : ANotifyPropertyChangedModel
    {
        public Progress<TaskUnitOfWork> Progress { get; set; } = new Progress<TaskUnitOfWork>();

        private readonly DispatcherTimer timer;

        public TaskOverviewViewModel()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        }

        public ObservableCollection<TaskViewModel> TaskModels { get; } = new ObservableCollection<TaskViewModel>();

        private ConcurrentDictionary<string, TaskViewModel> tasksMap { get; } = new ConcurrentDictionary<string, TaskViewModel>();

        private int minTasks = 0;
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

        private int executedTasks = int.MinValue;
        public int ExecutedTasks
        {
            get { return executedTasks; }
            set
            {
                if (executedTasks == value) { return; }

                executedTasks = value;
                NotifyPropertyChanged();
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

        internal void Update(TaskManager manager)
        {
            if (manager is null || manager.HasTasks == false) { return; }

            TaskModels.Clear();
            tasksMap.Clear();
            MaxTasks = 0;
            foreach (var unitOfWork in manager.UnitOfWorks)
            {
                var viewModel = new TaskViewModel
                {
                    AllExecutedFunc = AllTaskExecuted
                };
                viewModel.TaskUoW = unitOfWork;
                TaskModels.Add(viewModel);
                tasksMap.TryAdd(unitOfWork.TaskId, viewModel);
                MaxTasks += 1;
            }
            ExecutedTasks = 0;
        }

        private bool AllTaskExecuted()
        {
            return ExecutedTasks >= MaxTasks;
        }

        private void Progress_ProgressChanged(object sender, TaskUnitOfWork unitOfWork)
        {
            if (tasksMap.ContainsKey(unitOfWork.TaskId) == false
                || unitOfWork.Status.IsExecuted == false) { return; }

            ExecutedTasks = TaskModels.Count - tasksMap.Count;
            if (unitOfWork.Status.IsCleanedUp 
                && tasksMap.TryRemove(unitOfWork.TaskId, out var viewModel))
            {
                viewModel.RemoveTimer(timer);
                viewModel.RemoveProgessEvent(Progress);
            }
        }
    }
}
