using RevitJournal.Journal;
using System;
using System.ComponentModel;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI
{
    public class JournalTaskResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public const string NoMessage = "No";
        public const string YesMessage = "Yes";
        public const string TimeoutMessage = "Timeout";
        public const string RuntimeParameter = "RunningTime";

        private Visibility _ResultVisible = Visibility.Collapsed;
        public Visibility ResultVisible
        {
            get { return _ResultVisible; }
            set
            {
                if (_ResultVisible == value) { return; }

                _ResultVisible = value;
                OnPropertyChanged(nameof(ResultVisible));
            }
        }

        public void UpdateResult(JournalResult result)
        {
            if(result is null) { return; }

            if (result.Executed)
            {
                ResultVisible = Visibility.Visible;
            }
            ///TODO
            //if (result.HasError(out var error))
            //{
            //    ErrorText = YesMessage;
            //    ErrorTextToolTip = error.ErrorMessage;
            //}
            if (result.IsTimeout)
            {
                ExecutionTime = TimeoutMessage;
            }
        }

        internal void OnRunTimeChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(RuntimeParameter, StringComparison.CurrentCulture) == false) { return; }

            if (sender is JournalTaskExecuteViewModel model)
            {
                ExecutionTime = model.RunningTime + " / " + model.TimeoutTime;
            }
        }

        private string _ExecutionTime = string.Empty;
        public string ExecutionTime
        {
            get { return _ExecutionTime; }
            set
            {
                if (_ExecutionTime.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ExecutionTime = value;
                OnPropertyChanged(nameof(ExecutionTime));
            }
        }

        private string _ErrorText = NoMessage;
        public string ErrorText
        {
            get { return _ErrorText; }
            set
            {
                if (_ErrorText.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ErrorText = value;
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        private string _ErrorTextToolTip = string.Empty;
        public string ErrorTextToolTip
        {
            get { return _ErrorTextToolTip; }
            set
            {
                if (_ErrorTextToolTip.Equals(value, StringComparison.CurrentCulture)) { return; }

                _ErrorTextToolTip = value;
                OnPropertyChanged(nameof(ErrorTextToolTip));
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
