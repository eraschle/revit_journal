using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class ParameterCheckboxValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && bool.TryParse(value.ToString(), out var boolValue))
            {
                return boolValue;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                return value.ToString();
            }

            return string.Empty;
        }
    }
}
