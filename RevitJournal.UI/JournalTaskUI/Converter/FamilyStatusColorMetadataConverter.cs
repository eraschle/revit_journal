using DataSource.Metadata;
using System;
using System.Globalization;
using System.Windows.Media;

namespace RevitJournalUI.JournalTaskUI.Converter
{
    public class FamilyStatusColorMetadataConverter : AMetadataConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values is null || values.Length < 1 || !(values[0] is MetadataStatus status)) { return false; }

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

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
