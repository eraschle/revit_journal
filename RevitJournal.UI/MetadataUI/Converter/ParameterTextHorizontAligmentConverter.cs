using RevitJournalUI.MetadataUI.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class ParameterTextHorizontAligmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var horizontal = HorizontalAlignment.Center;
            if (value is ParameterFileMetadataViewModel model)
            {
                horizontal = model.HasUnit ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            }

            return horizontal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
