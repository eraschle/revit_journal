using RevitJournalUI.MetadataUI.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class ParameterCheckboxVisbilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ParameterFileMetadataViewModel model)) { return Visibility.Hidden; }

            if (bool.TryParse(model.Value, out _))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
