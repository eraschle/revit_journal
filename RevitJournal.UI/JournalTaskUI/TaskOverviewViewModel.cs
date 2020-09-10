using RevitJournal.Tasks;
using RevitJournalUI.Tasks;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace RevitJournalUI.JournalTaskUI
{
    public class TaskOverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Progress<TaskUnitOfWork> Progress { get; set; } = new Progress<TaskUnitOfWork>();


        private const string PrefixExecutedTask = "Executed Tasks ";

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
                OnPropertyChanged(nameof(MinTasks));
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
                OnPropertyChanged(nameof(MaxTasks));
            }
        }

        private int executedTasks = 0;
        public int ExecutedTasks
        {
            get { return executedTasks; }
            set
            {
                if (executedTasks == value) { return; }

                executedTasks = value;
                OnPropertyChanged(nameof(ExecutedTasks));
            }
        }

        private string executedTasksText = string.Empty;
        public string ExecutedTasksText
        {
            get { return executedTasksText; }
            set
            {
                if (executedTasksText.Equals(value, StringComparison.CurrentCulture)) { return; }

                executedTasksText = value;
                OnPropertyChanged(nameof(ExecutedTasksText));
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
            MaxTasks = 0;
            foreach (var unitOfWork in manager.UnitOfWorks)
            {
                MaxTasks += 1;
                var viewModel = new TaskViewModel { TaskUoW = unitOfWork };
                TaskModels.Add(viewModel);
            }

            ExecutedTasks = 0;
            SetExecutedTasks();
        }

        private void Progress_ProgressChanged(object sender, TaskUnitOfWork task)
        {
            if (task.Status.IsCleanUp == false) { return; }

            var viewModel = TaskModels.FirstOrDefault(mdl => mdl.TaskUoW.Equals(task));
            if (viewModel is null) { return; }

            else if (task.Status.IsCleanUp)
            {
                task.Cleanup();
                viewModel.RemoveTimer(timer);
                viewModel.RemoveProgessEvent(Progress);
                ExecutedTasks += 1;
                SetExecutedTasks();
            }
        }

        private void SetExecutedTasks()
        {
            var executed = ExecutedTasks + " / " + MaxTasks;
            ExecutedTasksText = PrefixExecutedTask + executed;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
