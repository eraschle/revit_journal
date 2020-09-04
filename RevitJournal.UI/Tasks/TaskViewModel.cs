using RevitJournal.Tasks;
using RevitJournalUI.JournalTaskUI;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace RevitJournalUI.Tasks
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string NoJournalProcess = "No Journal created";
        private const string NoJournalRevit = "No Revit Journal";

        internal TaskUnitOfWork TaskUoW { get; set; }

        public JournalTaskExecuteViewModel JournalTaskExecute { get; private set; }
        public JournalTaskResultViewModel JournalTaskResult { get; private set; }

        public TaskViewModel()
        {
            JournalTaskExecute = new JournalTaskExecuteViewModel();
            JournalTaskResult = new JournalTaskResultViewModel();
            JournalTaskExecute.PropertyChanged += new PropertyChangedEventHandler(JournalTaskResult.OnRunTimeChanged);
        }

        public string TaskName
        {
            get { return TaskUoW.Task.Name; }
        }

        private string taskStatus = string.Empty;
        public string TaskStatus
        {
            get { return taskStatus; }
            set
            {
                if (taskStatus.Equals(value, StringComparison.CurrentCulture)) { return; }

                taskStatus = value;
                OnPropertyChanged(nameof(TaskStatus));
            }
        }

        private SolidColorBrush taskStatusColor = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush TaskStatusColor
        {
            get { return taskStatusColor; }
            set
            {
                if (taskStatusColor.Equals(value)) { return; }

                taskStatusColor = value;
                OnPropertyChanged(nameof(TaskStatusColor));
            }
        }


        private string _JournalTask = NoJournalProcess;
        public string JournalTask
        {
            get { return _JournalTask; }
            set
            {
                if (_JournalTask.Equals(value, StringComparison.CurrentCulture)) { return; }

                _JournalTask = value;
                OnPropertyChanged(nameof(JournalTask));
            }
        }

        private string _JournalRecorde = NoJournalRevit;
        public string JournalRecorde
        {
            get { return _JournalRecorde; }
            set
            {
                if (_JournalRecorde.Equals(value, StringComparison.CurrentCulture)) { return; }

                _JournalRecorde = value;
                OnPropertyChanged(nameof(JournalRecorde));
            }
        }

        internal void AddProgessEvent()
        {
            TaskUoW.Progress.ProgressChanged += Progress_ProgressChanged;
            JournalTaskExecute.AddProgessEvent(TaskUoW);
            JournalTaskResult.AddProgessEvent(TaskUoW);
        }

        internal void RemoveProgessEvent()
        {
            TaskUoW.Progress.ProgressChanged -= Progress_ProgressChanged;
            JournalTaskExecute.RemoveProgessEvent(TaskUoW);
            JournalTaskResult.RemoveProgessEvent(TaskUoW);
        }

        private void Progress_ProgressChanged(object sender, TaskUnitOfWork task)
        {
            TaskStatusColor = GetStatusColor();
            TaskStatus = task.Status.GetStatusText();
            SetJournals();
        }

        private void SetJournals()
        {
            if (TaskUoW.HasTaskJournal)
            {
                JournalTask = TaskUoW.TaskJournal.NameWithExtension;
            }
            if (TaskUoW.HasRecordeJournal)
            {
                JournalRecorde = TaskUoW.RecordeJournal.NameWithExtension;
            }
        }

        private SolidColorBrush GetStatusColor()
        {
            if (TaskUoW.Status.IsWaiting)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else if (TaskUoW.Status.IsStarted)
            {
                return new SolidColorBrush(Colors.GreenYellow);
            }
            else if (TaskUoW.Status.Executed)
            {
                var color = new SolidColorBrush(Colors.Green);
                if (TaskUoW.Status.IsCancel)
                {
                    color = new SolidColorBrush(Colors.OrangeRed);
                }
                if (TaskUoW.Status.IsError)
                {
                    color = new SolidColorBrush(Colors.Red);
                }
                else if (TaskUoW.Status.IsTimeout)
                {
                    color = new SolidColorBrush(Colors.DarkRed);
                }
                return color;
            }
            else
            {
                return new SolidColorBrush(Colors.Cyan);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
