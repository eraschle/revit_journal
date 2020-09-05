using RevitAction.Report;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.Tasks.Converter
{
    public class TaskStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReportStatus status))
            {
                return string.Empty;
            }
            return status.GetStatusText();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
