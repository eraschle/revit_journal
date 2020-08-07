using RevitJournal.Journal;
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

        private int _MinTasks = 1;
        public int MinTasks
        {
            get { return _MinTasks; }
            set
            {
                if (_MinTasks == value) { return; }

                _MinTasks = value;
                OnPropertyChanged(nameof(MinTasks));
            }
        }

        private int _MaxTasks = 0;
        public int MaxTasks
        {
            get { return _MaxTasks; }
            set
            {
                if (_MaxTasks == value) { return; }

                _MaxTasks = value;
                OnPropertyChanged(nameof(MaxTasks));
            }
        }

        private int _ExecutedTasks = 0;
        public int ExecutedTasks
        {
            get { return _ExecutedTasks; }
            set
            {
                if (_ExecutedTasks == value) { return; }

                _ExecutedTasks = value;
                OnPropertyChanged(nameof(ExecutedTasks));
            }
        }

        private string _ExecutedTasksText = string.Empty;
        public string ExecutedTasksText
        {
            get { return _ExecutedTasksText; }
            set
            {
                if (_ExecutedTasksText.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ExecutedTasksText = value;
                OnPropertyChanged(nameof(ExecutedTasksText));
            }
        }

        internal void SetResult(JournalManager manager, JournalResult result)
        {
            SetExecutedTasks(manager);
            foreach (var viewModel in JournalTaskModels)
            {
                if (viewModel.IsJournalTask(result) == false) { continue; }

                viewModel.SetResult(result);
                break;
            }
        }

        private void SetExecutedTasks(JournalManager manager)
        {
            MaxTasks = manager.JournalTaskCount;
            ExecutedTasks = manager.JournalTaskExecutedCount;
            var executed = ExecutedTasks + " / " + MaxTasks;
            ExecutedTasksText = PrefixExecutedTask + executed;
        }

        public void Update(JournalManager manager)
        {
            if (manager is null) { return; }

            if (manager.HasJournalTasks == false) { return; }

            JournalTaskModels.Clear();
            foreach (var task in manager.JournalTasks)
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
