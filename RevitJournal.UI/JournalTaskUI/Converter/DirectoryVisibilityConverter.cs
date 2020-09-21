using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitJournalUI.JournalTaskUI.Converter
{
    public class DirectoryVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is int intValue) || intValue == 0 
                ? Visibility.Collapsed 
                : (object)Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
