using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.JournalTaskUI.Parameters
{
    public class JournalCommandParameterBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null) { value = bool.FalseString; }

            return TypeDescriptor.GetConverter(targetType).ConvertFrom(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TypeDescriptor.GetConverter(targetType).ConvertTo(value, typeof(string));
        }
    }
}
