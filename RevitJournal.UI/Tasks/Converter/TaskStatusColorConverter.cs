using RevitAction.Report;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitJournalUI.Tasks.Converter
{
    public class TaskStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReportStatus status))
            {
                return new SolidColorBrush(Colors.Cyan);
            }

            if (status.IsWaiting)
            {
                return new SolidColorBrush(Colors.Yellow);
            }
            else if (status.IsStarted)
            {
                return new SolidColorBrush(Colors.GreenYellow);
            }
            else if (status.Executed)
            {
                var color = new SolidColorBrush(Colors.Green);
                if (status.IsCancel)
                {
                    color = new SolidColorBrush(Colors.OrangeRed);
                }
                if (status.IsError)
                {
                    color = new SolidColorBrush(Colors.Red);
                }
                else if (status.IsTimeout)
                {
                    color = new SolidColorBrush(Colors.DarkRed);
                }
                return color;
            }
            return value is Control control ? control.Background : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
