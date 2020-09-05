using RevitAction.Report;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitJournalUI.Tasks.Converter
{
    public class TaskResultVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReportStatus status) || status.Executed == false)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
