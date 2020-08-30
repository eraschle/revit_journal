using RevitJournal.Journal;
using RevitJournal.Tasks;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace RevitJournalUI.JournalTaskUI
{
    public class JournalTaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const string NoJournalProcess = "No Journal created";
        private const string NoJournalRevit = "No Revit Journal";

        private readonly RevitTask RevitTask;

        public TaskReport Result { get; private set; }

        public JournalTaskExecuteViewModel JournalTaskExecute { get; private set; }
        public JournalTaskResultViewModel JournalTaskResult { get; private set; }

        public JournalTaskViewModel(RevitTask task)
        {
            RevitTask = task;
            JournalTaskExecute = new JournalTaskExecuteViewModel();
            JournalTaskResult = new JournalTaskResultViewModel();
            JournalTaskExecute.PropertyChanged += new PropertyChangedEventHandler(JournalTaskResult.OnRunTimeChanged);
        }

        public string JournalTaskName
        {
            get { return RevitTask.Name; }
        }

        private string _JournalStatus = string.Empty;
        public string JournalStatus
        {
            get { return _JournalStatus; }
            set
            {
                if (_JournalStatus.Equals(value, StringComparison.CurrentCulture)) { return; }

                _JournalStatus = value;
                OnPropertyChanged(nameof(JournalStatus));
            }
        }

        private SolidColorBrush _JournalStatusColor = new SolidColorBrush(Colors.Transparent);
        public SolidColorBrush JournalStatusColor
        {
            get { return _JournalStatusColor; }
            set
            {
                if (_JournalStatusColor.Equals(value)) { return; }

                _JournalStatusColor = value;
                OnPropertyChanged(nameof(JournalStatusColor));
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

        internal bool IsTask(TaskReport result)
        {
            return result != null && result.IsTask(RevitTask);
        }

        internal void SetResult(TaskReport result)
        {
            if (result is null) { return; }

            Result = result;
            SetStatus();
            SetJournals();
            JournalTaskExecute.UpdateResult(result);
            JournalTaskResult.UpdateResult(result);
        }

        private void SetJournals()
        {
            if (Result.HasTaskJournal)
            {
                JournalTask = Result.TaskJournal.NameWithExtension;
            }
            if (Result.HasRecordeJournal)
            {
                JournalRecorde = Result.RecordeJournal.NameWithExtension;
            }
        }
        private void SetStatus()
        {
            if (Result.Status.IsWaiting)
            {
                JournalStatus = "Wait";
                JournalStatusColor = new SolidColorBrush(Colors.Yellow);
            }
            else if (Result.Status.IsStarted)
            {
                JournalStatus = "Run";
                JournalStatusColor = new SolidColorBrush(Colors.GreenYellow);
            }
            else if (Result.Status.Executed)
            {
                JournalStatus = "Finish";
                JournalStatusColor = new SolidColorBrush(Colors.Green);
                if (Result.Status.IsCancel)
                {
                    JournalStatusColor = new SolidColorBrush(Colors.OrangeRed);
                }
                if (Result.Status.IsError)
                {
                    JournalStatusColor = new SolidColorBrush(Colors.Red);
                }
                else if (Result.Status.IsTimeout)
                {
                    JournalStatusColor = new SolidColorBrush(Colors.DarkRed);
                }
            }
            else
            {
                JournalStatus = "Unknown";
                JournalStatusColor = new SolidColorBrush(Colors.Cyan);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
