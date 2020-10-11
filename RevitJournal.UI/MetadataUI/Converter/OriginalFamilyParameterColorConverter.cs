using DataSource.Model.Metadata;
using RevitJournal.Duplicate.Comparer;
using System;
using System.Globalization;

namespace RevitJournalUI.MetadataUI.Converter
{
    public class OriginalFamilyParameterColorConverter : AColorConverter<Parameter>
    {
        public OriginalFamilyParameterColorConverter() : base(new ParameterDuplicateComparer()) { }

        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is null || values.Length == 0) { return ConverterColors.TransparentBackground; }

            var parameterName = values[0].ToString();

            var comparer = Comparer.ByName(parameterName);
            return ConverterColors.GetOriginalColor(comparer);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
