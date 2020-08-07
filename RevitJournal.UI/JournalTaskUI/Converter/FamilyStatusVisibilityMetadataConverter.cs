using DataSource.Metadata;
using System;
using System.Globalization;
using System.Windows;

namespace RevitJournalUI.JournalTaskUI.Converter
{
    public class FamilyStatusVisibilityMetadataConverter : AMetadataConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length < 1 || !(values[0] is MetadataStatus status)) { return false; }

            return status == MetadataStatus.Valid ? Visibility.Visible : Visibility.Hidden;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
