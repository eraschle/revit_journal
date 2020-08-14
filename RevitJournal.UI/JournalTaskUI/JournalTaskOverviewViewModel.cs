using RevitJournal.Journal;
using RevitJournal.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace RevitJournalUI.JournalTaskUI
{
    public class JournalTaskOverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string PrefixExecutedTask = "Executed Tasks ";

        public ObservableCollection<JournalTaskViewModel> JournalTaskModels { get; } = new ObservableCollection<JournalTaskViewModel>();

        public IEnumerable<JournalResult> JournalTaskResults
        {
            get { return JournalTaskModels.Select(model => model.Result); }
        }

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

        internal void SetResult(TaskManager manager, JournalResult result)
        {
            SetExecutedTasks(manager);
            foreach (var viewModel in JournalTaskModels)
            {
                if (viewModel.IsJournalTask(result) == false) { continue; }

                viewModel.SetResult(result);
                break;
            }
        }

        private void SetExecutedTasks(TaskManager manager)
        {
            MaxTasks = manager.JournalTaskCount;
            ExecutedTasks = manager.TaskExecutedCount;
            var executed = ExecutedTasks + " / " + MaxTasks;
            ExecutedTasksText = PrefixExecutedTask + executed;
        }

        public void Update(TaskManager manager)
        {
            if (manager is null) { return; }

            if (manager.HasRevitTasks == false) { return; }

            JournalTaskModels.Clear();
            foreach (var task in manager.RevitTasks)
            {
                JournalTaskModels.Add(new JournalTaskViewModel(task));
            }
            SetExecutedTasks(manager);
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
