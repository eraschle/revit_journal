using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.Converters
{
    public class TimespanMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is TimeSpan timeSpan ? (double)timeSpan.TotalMinutes : double.MinValue;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            return value is double timespan ? TimeSpan.FromMinutes(timespan) : TimeSpan.MinValue;
        }
    }
}
