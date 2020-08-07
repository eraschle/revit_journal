using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.JournalTaskUI.Converter
{
    public abstract class AMetadataConverter : IMultiValueConverter
    {
        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
    }
}
