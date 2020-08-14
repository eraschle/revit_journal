using RevitJournal.Journal;
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

        private readonly RevitTask JournalTask;

        public JournalResult Result { get; private set; }

        public JournalTaskExecuteViewModel JournalTaskExecute { get; private set; }
        public JournalTaskResultViewModel JournalTaskResult { get; private set; }

        public JournalTaskViewModel(RevitTask journalTask)
        {
            JournalTask = journalTask;
            JournalTaskExecute = new JournalTaskExecuteViewModel();
            JournalTaskResult = new JournalTaskResultViewModel();
            JournalTaskExecute.PropertyChanged += new PropertyChangedEventHandler(JournalTaskResult.OnRunTimeChanged);
        }

        public string JournalTaskName
        {
            get { return JournalTask.Name; }
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


        private string _JournalProcess = NoJournalProcess;
        public string JournalProcess
        {
            get { return _JournalProcess; }
            set
            {
                if (_JournalProcess.Equals(value, StringComparison.CurrentCulture)) { return; }

                _JournalProcess = value;
                OnPropertyChanged(nameof(JournalProcess));
            }
        }

        private string _JournalRevit = NoJournalRevit;
        public string JournalRevit
        {
            get { return _JournalRevit; }
            set
            {
                if (_JournalRevit.Equals(value, StringComparison.CurrentCulture)) { return; }

                _JournalRevit = value;
                OnPropertyChanged(nameof(JournalRevit));
            }
        }

        internal bool IsJournalTask(JournalResult result)
        {
            return JournalTask.Equals(result.JournalTask);
        }

        internal void SetResult(JournalResult result)
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
            if (Result.HasJournalProcess)
            {
                JournalProcess = Result.JournalProcess.NameWithExtension;
            }
            if (Result.HasJournalRevit)
            {
                JournalRevit = Result.JournalRevit.NameWithExtension;
            }
        }
        private void SetStatus()
        {
            if (Result.IsWaiting)
            {
                JournalStatus = "Wait";
                JournalStatusColor = new SolidColorBrush(Colors.Yellow);
            }
            else if (Result.IsStarted)
            {
                JournalStatus = "Run";
                JournalStatusColor = new SolidColorBrush(Colors.GreenYellow);
            }
            else if (Result.Executed)
            {
                JournalStatus = "Finish";
                JournalStatusColor = new SolidColorBrush(Colors.Green);
                if (Result.IsCancel)
                {
                    JournalStatusColor = new SolidColorBrush(Colors.OrangeRed);
                }
                if (Result.IsError)
                {
                    JournalStatusColor = new SolidColorBrush(Colors.Red);
                }
                else if (Result.IsTimeout)
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
