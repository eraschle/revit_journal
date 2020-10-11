using System.Windows;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI
{
    public class StatusBarViewModel : ANotifyPropertyChangedModel
    {
        private static StatusBarViewModel instane;

        public static StatusBarViewModel Instance
        {
            get
            {
                if (instane is null)
                {
                    instane = new StatusBarViewModel();
                    instane.Reset();
                }
                return instane;
            }
        }

        private StatusBarViewModel() { }

        private string status = "Ok";
        public string Status
        {
            get { return status; }
            set
            {
                if (StringUtils.Equals(status, value)) { return; }

                status = value;
                NotifyPropertyChanged();
            }
        }

        private int minValue = 0;
        public int MinValue
        {
            get { return minValue; }
            set
            {
                if (minValue == value) { return; }

                minValue = value;
                NotifyPropertyChanged();
            }
        }

        private string progressText = string.Empty;
        public string ProgressText
        {
            get { return progressText; }
            set
            {
                if (StringUtils.Equals(progressText, value)) { return; }

                progressText = value;
                NotifyPropertyChanged();
            }
        }

        private int progressValue = 0;
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                if (progressValue == value) { return; }

                progressValue = value;
                NotifyPropertyChanged();
            }
        }

        private int maxValue = 100;
        public int MaxValue
        {
            get { return maxValue; }
            set
            {
                if (maxValue == value) { return; }

                maxValue = value;
                NotifyPropertyChanged();
            }
        }

        private Visibility progressVisibility = Visibility.Hidden;
        public Visibility ProgressVisibility
        {
            get { return progressVisibility; }
            set
            {
                if (progressVisibility == value) { return; }

                progressVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool navigationEnabled = true;
        public bool NavigationEnabled
        {
            get { return navigationEnabled; }
            set
            {
                if (navigationEnabled == value) { return; }

                navigationEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public void SetPercentage(string status)
        {
            Status = status;
            NavigationEnabled = false;
            ProgressVisibility = Visibility.Visible;
            ProgressText = string.Empty;
            MinValue = 0;
            ProgressValue = 0;
            MaxValue = 100;
        }

        public void Reset()
        {
            Status = "Ok";
            NavigationEnabled = true;
            ProgressVisibility = Visibility.Hidden;
            ProgressText = string.Empty;
            MinValue = 0;
            ProgressValue = 0;
            MaxValue = 100;
        }
    }
}
