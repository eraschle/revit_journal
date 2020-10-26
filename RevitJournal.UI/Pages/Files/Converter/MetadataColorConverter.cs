using DataSource.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RevitJournalUI.Pages.Files.Converter
{
    public class MetadataColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length < 1 || !(values[0] is MetadataStatus status)) { return false; }

            switch (status)
            {
                case MetadataStatus.Valid:
                    return new SolidColorBrush(Colors.Green);
                case MetadataStatus.Repairable:
                    return new SolidColorBrush(Colors.Orange);
                case MetadataStatus.Error:
                    return new SolidColorBrush(Colors.Red);
                default:
                    return new SolidColorBrush(Colors.WhiteSmoke);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
