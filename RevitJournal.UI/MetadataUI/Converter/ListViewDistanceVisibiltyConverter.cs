using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class ListViewDistanceVisibiltyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var visibilty = Visibility.Visible;
            if(values != null && values.Length > 0)
            {
                var value = values[0].ToString();
                visibilty = string.IsNullOrWhiteSpace(value) ? Visibility.Hidden : Visibility.Visible;
            }
            return visibilty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
