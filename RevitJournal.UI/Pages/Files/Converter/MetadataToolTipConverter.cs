using DataSource.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RevitJournalUI.Pages.Files.Converter
{
    public class MetadataToolTipConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length < 1 || !(values[0] is MetadataStatus status)) { return false; }

            return Metadata.GetStatusName(status);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}