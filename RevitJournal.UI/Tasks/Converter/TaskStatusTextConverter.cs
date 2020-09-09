using RevitAction;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.Tasks.Converter
{
    public class TaskStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TaskAppStatus status))
            {
                return string.Empty;
            }

            if (status.IsInitial)
            {
                return "Initial";
            }
            if (status.IsWaiting)
            {
                return "Wait";
            }
            if (status.IsStarted)
            {
                return "Start";
            }
            if (status.IsOpen)
            {
                return "Open";
            }
            if (status.IsRun)
            {
                return "Run";
            }
            if (status.IsError)
            {
                return "Error";
            }
            if (status.IsTimeout)
            {
                return "Timeout";
            }
            if (status.IsCancel)
            {
                return "Cancel";
            }
            return "Finished";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
