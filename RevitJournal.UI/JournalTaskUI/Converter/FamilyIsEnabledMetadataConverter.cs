using DataSource.Model;
using System;
using System.Globalization;

namespace RevitJournalUI.JournalTaskUI.Converter
{
    public class FamilyIsEnabledMetadataConverter : AMetadataConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values is null || values.Length < 1 || !(values[0] is MetadataStatus status)) { return false; }

            if(status == MetadataStatus.Valid) { return true; }
            
            return false;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
