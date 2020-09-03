using RevitJournal.Tasks.Report;
using RevitJournalUI.Helper;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace RevitJournalUI.JournalTaskUI
{
    public class JournalTaskExecuteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TimeSpan runTime = TimeSpan.Zero;

        private readonly DispatcherTimer timer;

        public JournalTaskExecuteViewModel()
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += new EventHandler(DispatcherTimer_Tick);
        }

        private TaskReport Result;

        private Visibility executeVisible = Visibility.Collapsed;
        public Visibility ExecuteVisible
        {
            get { return executeVisible; }
            set
            {
                if (executeVisible == value) { return; }

                executeVisible = value;
                OnPropertyChanged(nameof(ExecuteVisible));
            }
        }

        public void UpdateResult(TaskReport result)
        {
            ///TODO refactor Progress
            if (result is null) { return; }

            Result = result;
            if (timer.IsEnabled == false && Result.Status.IsStarted)
            {
                runTime = timer.Interval;
                SetRuning();
                timer.Start();
                ExecuteVisible = Visibility.Visible;
            }
            if (Result.Status.Executed)
            {
                timer.Stop();
                if (Result.Status.IsTimeout)
                {
                    ///TODO
                    //runTime = Result.ProcessTimeout;
                }
                SetRuning();
                ExecuteVisible = Visibility.Collapsed;
            }
        }

        private string timeoutTime = string.Empty;
        public string TimeoutTime
        {
            get { return timeoutTime; }
            set
            {
                if (timeoutTime.Equals(value, StringComparison.CurrentCulture)) { return; }

                timeoutTime = value;
                OnPropertyChanged(nameof(TimeoutTime));
            }
        }

        private double _RunningPercent;
        public double RunningPercent
        {
            get { return _RunningPercent; }
            set
            {
                _RunningPercent = value;
                OnPropertyChanged(nameof(RunningPercent));
            }
        }

        private string _RunningPercentText = string.Empty;
        public string RunningPercentText
        {
            get { return _RunningPercentText; }
            set
            {
                if (_RunningPercentText.Equals(value, StringComparison.CurrentCulture)) { return; }

                _RunningPercentText = value;
                OnPropertyChanged(nameof(RunningPercentText));
            }
        }

        private string _RunningTime = string.Empty;
        public string RunningTime
        {
            get { return _RunningTime; }
            set
            {
                if (_RunningTime.Equals(value, StringComparison.CurrentCulture)) { return; }

                _RunningTime = value;
                OnPropertyChanged(nameof(RunningTime));
            }
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Result.Status.IsStarted)
            {
                runTime += timer.Interval;
                SetRuning();
            }
        }

        private void SetRuning()
        {
            if (Result is null) { return; }
            ///TODO
            //TimeoutTime = TimeSpanHelper.GetMinuteAndSeconds(Result.ProcessTimeout);
            RunningTime = TimeSpanHelper.GetMinuteAndSeconds(runTime);
            ///TODO
            //RunningPercent = runTime.TotalSeconds * 100 / Result.ProcessTimeout.TotalSeconds;
            if (RunningPercent > 1)
            {
                RunningPercentText = (RunningPercent / 100).ToString("#0.0 %", CultureInfo.CurrentCulture);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
