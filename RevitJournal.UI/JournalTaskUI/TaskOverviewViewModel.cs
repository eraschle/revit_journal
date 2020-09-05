﻿using RevitJournal.Tasks;
using RevitJournalUI.Tasks;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace RevitJournalUI.JournalTaskUI
{
    public class TaskOverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string PrefixExecutedTask = "Executed Tasks ";

        private readonly DispatcherTimer timer;

        public TaskOverviewViewModel()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Start();
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

        internal void AddProgessEvent()
        {
            foreach (var viewModel in TaskModels)
            {
                viewModel.TaskUoW.Progress.ProgressChanged += Progress_ProgressChanged;
                viewModel.AddProgessEvent();
            }
        }

        internal void RemoveProgessEvent()
        {
            foreach (var viewModel in TaskModels)
            {
                viewModel.TaskUoW.Progress.ProgressChanged += Progress_ProgressChanged;
                viewModel.RemoveProgessEvent();
            }
        }

        internal void Update(TaskManager manager)
        {
            ///TODO refactor Progress
            if (manager is null || manager.HasTasks == false) { return; }

            TaskModels.Clear();
            MaxTasks = 0;
            foreach (var unitOfWork in manager.UnitOfWorks)
            {
                MaxTasks += 1;
                var viewModel = new TaskViewModel { TaskUoW = unitOfWork };
                viewModel.AddTimer(timer);
                TaskModels.Add(viewModel);
            }
        }

        private void Progress_ProgressChanged(object sender, TaskUnitOfWork task)
        {
            if (task.Status.Executed == false) { return; }

            //if finished count plus 1 
            var executed = ExecutedTasks + " / " + MaxTasks;
            ExecutedTasksText = PrefixExecutedTask + executed;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
