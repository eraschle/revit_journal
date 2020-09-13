using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitJournalUI.Tasks.Converter
{
    public class TaskStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is RevitAction.TaskAppStatus status))
            {
                return new SolidColorBrush(Colors.Cyan);
            }

            if (status.IsWaiting || status.IsInitial)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else if (status.IsStarted || status.IsRunning)
            {
                return new SolidColorBrush(Colors.GreenYellow);
            }
            else if (status.IsExecuted)
            {
                if (status.IsCancel)
                {
                    return new SolidColorBrush(Colors.OrangeRed);
                }
                if (status.IsError)
                {
                    return new SolidColorBrush(Colors.Red);
                }
                else if (status.IsTimeout)
                {
                    return new SolidColorBrush(Colors.DarkRed);
                }
                return new SolidColorBrush(Colors.Green);
            }
            return value is Control control ? control.Background : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
