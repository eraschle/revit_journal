using RevitJournal.Journal;
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

        private TimeSpan RunTime = TimeSpan.Zero;

        private readonly DispatcherTimer Timer;

        public JournalTaskExecuteViewModel()
        {
            Timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            Timer.Tick += new EventHandler(DispatcherTimer_Tick);
        }

        private JournalResult Result;

        private Visibility _ExecuteVisible = Visibility.Collapsed;
        public Visibility ExecuteVisible
        {
            get { return _ExecuteVisible; }
            set
            {
                if (_ExecuteVisible == value) { return; }

                _ExecuteVisible = value;
                OnPropertyChanged(nameof(ExecuteVisible));
            }
        }

        public void UpdateResult(JournalResult result)
        {
            if(result is null) { return; }

            Result = result;
            if (Timer.IsEnabled == false && Result.IsStarted)
            {
                RunTime = Timer.Interval;
                SetRuning();
                Timer.Start();
                ExecuteVisible = Visibility.Visible;
            }
            if (Result.Executed)
            {
                Timer.Stop();
                if (Result.IsTimeout)
                {
                    RunTime = Result.ProcessTimeout;
                }
                SetRuning();
                ExecuteVisible = Visibility.Collapsed;
            }
        }

        private string _TimeoutTime = string.Empty;
        public string TimeoutTime
        {
            get { return _TimeoutTime; }
            set
            {
                if (_TimeoutTime.Equals(value, StringComparison.CurrentCulture)) { return; }

                _TimeoutTime = value;
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
            if (Result.IsStarted)
            {
                RunTime += Timer.Interval;
                SetRuning();
            }
        }

        private void SetRuning()
        {
            if (Result is null) { return; }

            TimeoutTime = TimeSpanHelper.GetMinuteAndSeconds(Result.ProcessTimeout);
            RunningTime = TimeSpanHelper.GetMinuteAndSeconds(RunTime);
            RunningPercent = RunTime.TotalSeconds * 100 / Result.ProcessTimeout.TotalSeconds;
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
