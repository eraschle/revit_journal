using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.Pages.Settings
{
    public class OptionSliderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                value = timeSpan.TotalMinutes;
            }
            else if (value is int intValue)
            {
                value = (double)intValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            if (targetTypes == typeof(TimeSpan))
            {
                value = TimeSpan.FromMinutes((double)value);
            }
            else if (targetTypes == typeof(int))
            {
                value = (int)value;
            }
            return value;
        }
    }
}
